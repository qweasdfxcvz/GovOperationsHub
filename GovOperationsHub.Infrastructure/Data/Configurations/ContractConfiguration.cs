using GovOperationsHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Infrastructure.Data.Configurations
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts");

            builder.Property(c => c.ContractNumber).IsRequired().HasMaxLength(50);
            builder.HasIndex(c => c.ContractNumber).IsUnique();
            builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
            builder.Property(c => c.TotalBudget).HasPrecision(18, 2);

            builder.HasOne(c => c.Ministry)
                .WithMany(m => m.Contracts)
                .HasForeignKey(c => c.MinistryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.GovernmentEntity)
                .WithMany(g => g.Contracts)
                .HasForeignKey(c => c.GovernmentEntityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ContractAnnexConfiguration : IEntityTypeConfiguration<ContractAnnex>
    {
        public void Configure(EntityTypeBuilder<ContractAnnex> builder)
        {
            builder.ToTable("ContractAnnexes");

            builder.Property(a => a.AnnexNumber).IsRequired().HasMaxLength(50);
            builder.Property(a => a.ScopeDescription).HasMaxLength(1000);

            builder.HasOne(a => a.Contract)
                .WithMany(c => c.Annexes)
                .HasForeignKey(a => a.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
