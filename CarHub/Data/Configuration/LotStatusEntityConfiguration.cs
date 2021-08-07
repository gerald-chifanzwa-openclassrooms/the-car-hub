using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarHub.Data.Configuration
{
    public class LotStatusEntityConfiguration : IEntityTypeConfiguration<LotStatus>
    {
        public void Configure(EntityTypeBuilder<LotStatus> builder)
        {
            builder.ToTable("LotStatus");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();
            builder.Property(p => p.VehicleId).IsRequired();
            builder.Property(p => p.DateAdded).IsRequired().HasColumnType("DATE");
            builder.Property(p => p.SellDate).IsRequired(false).HasColumnType("DATE");
            builder.Property(p => p.SellingPrice).IsRequired().HasPrecision(10, 4);

            builder.HasOne(p => p.Vehicle).WithOne().HasForeignKey<LotStatus>(p => p.VehicleId);
        }
    }
}
