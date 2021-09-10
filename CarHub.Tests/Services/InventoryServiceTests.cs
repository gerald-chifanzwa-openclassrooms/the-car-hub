using System;
using System.Linq;
using Bogus;
using CarHub.Data;
using CarHub.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CarHub.Tests
{
    public class InventoryServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        public InventoryServiceTests()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbContextOptionsBuilder.UseInMemoryDatabase(nameof(InventoryServiceTests));
            _dbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options);
            _dbContext.Database.EnsureCreated();
        }
        private InventoryService GetInventoryService()
        {
            var logger = new NullLogger<InventoryService>();
            return new InventoryService(_dbContext, logger);
        }

        [Fact]
        public void GetVehicles_WhenUserIsNotAuthenticated_ShouldOnlyReturnPublishedVehicles()
        {
            // Arrange
            var inventoryService = GetInventoryService();
            var makesFaker = new Faker<VehicleMake>();
            makesFaker.RuleFor(m => m.Name, f => f.Vehicle.Manufacturer());

            var makes = makesFaker.Generate(10).ToArray();
            _dbContext.VehicleMakes.AddRange(makes);
            _dbContext.SaveChanges();

            Faker<Vehicle> faker = new();
            faker.RuleSet("Show", rules => InitializeVehicleGenerationRules(rules, makes, LotDisplayStatus.Show));

            faker.RuleSet("Hide", rules => InitializeVehicleGenerationRules(rules, makes, LotDisplayStatus.Hidden));
            var vehicles = faker.Generate(5, "Show").Concat(faker.Generate(10, "Hide"));

            _dbContext.Vehicles.AddRange(vehicles);
            _dbContext.SaveChanges();

            // Act
            var results = inventoryService.GetAllVehiclesAsync(1, 100, false, default).GetAwaiter().GetResult();

            // Assert
            results.Items.Should().HaveCount(5);
            results.Items.Should().OnlyContain(v => v.Status == LotDisplayStatus.Show);
        }

        [Fact]
        public void GetVehicles_WhenUserIsAuthenticated_ShouldOnlyReturnAllVehicles()
        {
            // Arrange
            var inventoryService = GetInventoryService();
            var makesFaker = new Faker<VehicleMake>();
            makesFaker.RuleFor(m => m.Name, f => f.Vehicle.Manufacturer());

            var makes = makesFaker.Generate(10).ToArray();
            _dbContext.VehicleMakes.AddRange(makes);
            _dbContext.SaveChanges();

            Faker<Vehicle> faker = new();
            faker.RuleSet("Show", rules => InitializeVehicleGenerationRules(rules, makes, LotDisplayStatus.Show));

            faker.RuleSet("Hide", rules => InitializeVehicleGenerationRules(rules, makes, LotDisplayStatus.Hidden));
            var vehicles = faker.Generate(5, "Show").Concat(faker.Generate(10, "Hide"));

            _dbContext.Vehicles.AddRange(vehicles);
            _dbContext.SaveChanges();

            // Act
            var results = inventoryService.GetAllVehiclesAsync(1, 100, true, default).GetAwaiter().GetResult();

            // Assert
            results.Items.Should().HaveCount(15);
        }

        [Fact]
        public void GetVehicle_ShouldReturnVehicle()
        {
            // Arrange
            var inventoryService = GetInventoryService();
            var makesFaker = new Faker<VehicleMake>();
            makesFaker.RuleFor(m => m.Name, f => f.Vehicle.Manufacturer());

            var makes = makesFaker.Generate(10).ToArray();
            var faker = new Faker<Vehicle>();
            faker.RuleFor(v => v.Model, f => f.Vehicle.Model());
            faker.RuleFor(v => v.Year, f => f.Date.Past().Year);
            faker.RuleFor(v => v.Make, f => f.Random.ArrayElement(makes));
            faker.RuleFor(v => v.PurchaseDate, f => f.Date.Recent());
            faker.RuleFor(v => v.PurchasePrice, f => f.Finance.Random.Number(1000, 10000));
            faker.RuleFor(v => v.Trim, f => f.Vehicle.Type());
            faker.RuleFor(v => v.Status, (f, v) => new LotStatus
            {
                SellDate = null,
                DateAdded = v.PurchaseDate,
                SellingPrice = v.PurchasePrice + 500,
                Status = LotDisplayStatus.Show
            });

            var vehicles = faker.Generate(5);

            _dbContext.Vehicles.AddRange(vehicles);
            _dbContext.SaveChanges();

            // Act
            var result = inventoryService.GetVehicleAsync(1, default).GetAwaiter().GetResult();

            // Assert
            result.Should().NotBeNull();
        }


        private static void InitializeVehicleGenerationRules(IRuleSet<Vehicle> rules, VehicleMake[] makes, LotDisplayStatus status)
        {
            rules.RuleFor(v => v.Status, (f, v) => new LotStatus
            {
                SellDate = null,
                DateAdded = v.PurchaseDate,
                SellingPrice = v.PurchasePrice + 500,
                Status = status
            });
            rules.RuleFor(v => v.Model, f => f.Vehicle.Model());
            rules.RuleFor(v => v.Year, f => f.Date.Past().Year);
            rules.RuleFor(v => v.MakeId, f => f.Random.ArrayElement(makes).Id);
            rules.RuleFor(v => v.PurchaseDate, f => f.Date.Recent());
            rules.RuleFor(v => v.PurchasePrice, f => f.Finance.Random.Number(1000, 10000));
            rules.RuleFor(v => v.Trim, f => f.Vehicle.Type());
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
