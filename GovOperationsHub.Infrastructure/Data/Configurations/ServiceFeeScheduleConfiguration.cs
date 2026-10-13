using GovOperationsHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Infrastructure.Data.Configurations
{
    public class ServiceFeeScheduleConfiguration : IEntityTypeConfiguration<ServiceFeeSchedule>
    {
        public void Configure(EntityTypeBuilder<ServiceFeeSchedule> builder)
        {
            builder.ToTable("ServiceFeeSchedules");

            builder.Property(f => f.DirectFee).HasPrecision(18, 2);
            builder.Property(f => f.IndirectFee).HasPrecision(18, 2);
            builder.Property(f => f.OurServiceMargin).HasPrecision(18, 2);
            builder.Property(f => f.OfficialDecreeNumber).HasMaxLength(100);

            builder.HasOne(f => f.GovernmentService)
                .WithMany(s => s.FeeSchedules)
                .HasForeignKey(f => f.GovernmentServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
