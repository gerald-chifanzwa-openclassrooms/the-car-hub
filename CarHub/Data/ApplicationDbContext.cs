using CarHub.Data.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<RepairDetail> Repairs => Set<RepairDetail>();
        public DbSet<VehicleMake> VehicleMakes => Set<VehicleMake>();
        public DbSet<VehicleImage> VehicleImages => Set<VehicleImage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new VehicleEntityConfiguration());
            builder.ApplyConfiguration(new RepairDetailEntityConfiguration());
            builder.ApplyConfiguration(new VehicleMakeEntityConfiguration());
            builder.ApplyConfiguration(new LotStatusEntityConfiguration());
            builder.ApplyConfiguration(new VehicleImageEntityConfiguration());

        }
    }
}
