using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarHub.Data.Configuration
{
    public class VehicleImageEntityConfiguration : IEntityTypeConfiguration<VehicleImage>
    {
        public void Configure(EntityTypeBuilder<VehicleImage> builder)
        {
            builder.ToTable("Images");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();
            builder.Property(p => p.FileName).IsUnicode(false).IsRequired().HasMaxLength(100);
            builder.Property(p => p.VehicleId).IsRequired();
            builder.Property(p => p.MimeType).IsUnicode(false).IsRequired().HasMaxLength(20);
            builder.Property(p => p.ImageData).IsRequired().HasColumnType("VARBINARY(MAX)");

            builder.HasOne(p => p.Vehicle).WithMany(p => p.Images).HasForeignKey(p => p.VehicleId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
