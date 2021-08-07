﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarHub.Data.Configuration
{
    public class VehicleEntityConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();
            builder.Property(p => p.MakeId).IsRequired();
            builder.Property(p => p.Model).IsRequired();
            builder.Property(p => p.Trim).IsRequired(false);
            builder.Property(p => p.PurchaseDate).IsRequired().HasColumnType("DATE");
            builder.Property(p => p.PurchasePrice).IsRequired().HasPrecision(10, 4);

            builder.HasOne(p => p.Make).WithMany().HasForeignKey(p => p.MakeId);
        }
    }
}
