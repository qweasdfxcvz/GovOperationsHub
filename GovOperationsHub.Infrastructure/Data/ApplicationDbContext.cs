using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Identity;
using GovOperationsHub.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;


namespace GovOperationsHub.Infrastructure.Data
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IEncryptionService _encryptionService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IEncryptionService encryptionService) : base(options)
        {
            _currentUserService = currentUserService;
            _encryptionService = encryptionService;
        }

        public DbSet<Ministry> Ministries => Set<Ministry>();
        public DbSet<GovernmentEntity> GovernmentEntities => Set<GovernmentEntity>();
        public DbSet<CitizenServiceCenter> CitizenServiceCenters => Set<CitizenServiceCenter>();
        public DbSet<ContactRecord> ContactRecords => Set<ContactRecord>();
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<ContractAnnex> ContractAnnexes => Set<ContractAnnex>();
        public DbSet<GovernmentService> GovernmentServices => Set<GovernmentService>();
        public DbSet<ServiceFeeSchedule> ServiceFeeSchedules => Set<ServiceFeeSchedule>();
        public DbSet<NetworkConfiguration> NetworkConfigurations => Set<NetworkConfiguration>();
        public DbSet<SlaStageTracker> SlaStageTrackers => Set<SlaStageTracker>();
        public DbSet<UserSession> UserSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تطبيق إعدادات الـ Fluent API
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // تمرير خدمة التشفير لتكوين NetworkConfigurationConfiguration
            modelBuilder.ApplyConfiguration(new NetworkConfigurationConfiguration(_encryptionService));

            // تطبيق Global Query Filter لجميع الكيانات الموروثة من BaseAuditableEntity لاستثناء السجلات المحذوفة (Soft Delete)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "p");
                    var property = Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));
                    var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }

            // تنظيم جداول الحسابات ضمن Schema خاص باسم "Security" لترتيب قاعدة البيانات
            modelBuilder.Entity<IdentityUser>().ToTable("Users", "Security");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", "Security");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Security");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Security");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Security");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Security");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Security");

            modelBuilder.Entity<UserSession>().ToTable("UserSessions", "Security");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var currentUser = _currentUserService.UserName ?? "System";

            foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = currentUser;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = now;
                        entry.Entity.LastModifiedBy = currentUser;
                        break;

                    case EntityState.Deleted:
                        // تحويل الحذف الفيزيائي إلى حذف منطقي Soft Delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.LastModifiedAt = now;
                        entry.Entity.LastModifiedBy = currentUser;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
