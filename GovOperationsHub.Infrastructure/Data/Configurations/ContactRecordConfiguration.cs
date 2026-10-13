using GovOperationsHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Infrastructure.Data.Configurations
{
    public class ContactRecordConfiguration : IEntityTypeConfiguration<ContactRecord>
    {
        public void Configure(EntityTypeBuilder<ContactRecord> builder)
        {
            builder.ToTable("ContactRecords");

            builder.Property(c => c.NameOrChannelTitle).IsRequired().HasMaxLength(150);
            builder.Property(c => c.JobTitle).HasMaxLength(100);
            builder.Property(c => c.DepartmentName).HasMaxLength(100);
            builder.Property(c => c.LandlinePhone).HasMaxLength(30);
            builder.Property(c => c.MobileNumber).HasMaxLength(30);
            builder.Property(c => c.Email).HasMaxLength(100);

            // منع الحذف التلقائي المتتالي لحماية البيانات من الحذف العرضي وتجنب Cascade Cycle
            builder.HasOne(c => c.Ministry)
                .WithMany(m => m.Contacts)
                .HasForeignKey(c => c.MinistryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.GovernmentEntity)
                .WithMany(g => g.Contacts)
                .HasForeignKey(c => c.GovernmentEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CitizenServiceCenter)
                .WithMany(csc => csc.Contacts)
                .HasForeignKey(c => c.CitizenServiceCenterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
