using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarHub.Data.Configuration
{
    public class RepairDetailEntityConfiguration : IEntityTypeConfiguration<RepairDetail>
    {
        public void Configure(EntityTypeBuilder<RepairDetail> builder)
        {
            builder.ToTable("Repairs");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();
            builder.Property(p => p.Cost).IsRequired().HasPrecision(10, 4);
            builder.Property(p => p.Date).IsRequired().HasColumnType("DATE");
            builder.Property(p => p.Description).IsRequired().HasMaxLength(200);
            builder.Property(p => p.VehicleId).IsRequired();
            builder.HasOne(p => p.Vehicle).WithMany(v => v.Repairs).HasForeignKey(p => p.VehicleId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
