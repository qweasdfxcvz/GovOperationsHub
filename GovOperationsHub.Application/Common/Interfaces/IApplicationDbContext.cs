using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Ministry> Ministries { get; }
        DbSet<GovernmentEntity> GovernmentEntities { get; }
        DbSet<CitizenServiceCenter> CitizenServiceCenters { get; }
        DbSet<ContactRecord> ContactRecords { get; }
        DbSet<BankAccount> BankAccounts { get; }
        DbSet<Contract> Contracts { get; }
        DbSet<ContractAnnex> ContractAnnexes { get; }
        DbSet<GovernmentService> GovernmentServices { get; }
        DbSet<ServiceFeeSchedule> ServiceFeeSchedules { get; }
        DbSet<NetworkConfiguration> NetworkConfigurations { get; }
        DbSet<SlaStageTracker> SlaStageTrackers { get; }
        DbSet<UserSession> UserSessions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
