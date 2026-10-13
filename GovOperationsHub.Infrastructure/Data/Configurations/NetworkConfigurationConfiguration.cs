using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Infrastructure.Data.Configurations
{
    public class NetworkConfigurationConfiguration : IEntityTypeConfiguration<NetworkConfiguration>
    {
        private readonly IEncryptionService _encryptionService;

        public NetworkConfigurationConfiguration(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public void Configure(EntityTypeBuilder<NetworkConfiguration> builder)
        {
            builder.ToTable("NetworkConfigurations");

            builder.Property(n => n.IpAddress).IsRequired().HasMaxLength(50);
            builder.Property(n => n.Gateway).HasMaxLength(50);
            builder.Property(n => n.RouterHardwareType).HasMaxLength(100);
            builder.Property(n => n.DedicatedCenterUrl).HasMaxLength(250);

            // تشفير الحقول الحساسة بواسطة محول التشفير
            var encryptionConverter = new EncryptedStringConverter(_encryptionService);

            builder.Property(n => n.EncryptedDatabasePassword)
                .HasConversion(encryptionConverter)
                .IsRequired();

            builder.Property(n => n.EncryptedRouterCredentials)
                .HasConversion(encryptionConverter);

            builder.HasOne(n => n.CitizenServiceCenter)
                .WithMany(c => c.NetworkConfigurations)
                .HasForeignKey(n => n.CitizenServiceCenterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
