using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarHub.Data.Configuration
{
    public class VehicleMakeEntityConfiguration : IEntityTypeConfiguration<VehicleMake>
    {
        public void Configure(EntityTypeBuilder<VehicleMake> builder)
        {
            builder.ToTable("VehicleMakes");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();
            builder.Property(p => p.Name).IsRequired().IsUnicode(false).HasMaxLength(20);
        }
    }
}
