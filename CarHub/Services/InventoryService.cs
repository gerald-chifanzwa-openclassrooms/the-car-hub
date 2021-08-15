using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Data;
using CarHub.Exceptions;
using CarHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarHub.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<InventoryService> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IEnumerable<VehicleViewModel>> GetAllVehiclesAsync(CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var query = _dbContext.Vehicles
                                                   .Include(v => v.Status)
                                                   .Include(v => v.Make)
                                                   .Where(v => v.Status.Status != LotDisplayStatus.Sold);
            if (!user.Identity.IsAuthenticated)
            {
                query = query.Where(v => v.Status.Status != LotDisplayStatus.Hidden);
            }
            var vehicles = await query.Select(v => new VehicleViewModel
            {
                Id = v.Id,
                Make = v.Make.Name,
                Model = v.Model,
                Trim = v.Trim,
                Year = v.Year,
                PurchaseDate = v.PurchaseDate,
                PurchasePrice = v.PurchasePrice,
                MakeId = v.Make.Id,
            }).ToListAsync(cancellationToken);

            return vehicles;
        }

        public async Task<VehicleDetailsViewModel> GetVehicleAsync(int vehicleId, CancellationToken cancellationToken)
        {
            var vehicleEntity = await _dbContext.Vehicles
                                                   .Include(v => v.Status)
                                                   .Include(v => v.Make)
                                                   .Include(v => v.Images)
                                                   .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

            return vehicleEntity == null
                ? null
                : new VehicleDetailsViewModel
                {
                    Id = vehicleEntity.Id,
                    Make = vehicleEntity.Make.Name,
                    Model = vehicleEntity.Model,
                    Trim = vehicleEntity.Trim,
                    Year = vehicleEntity.Year,
                    PurchaseDate = vehicleEntity.PurchaseDate,
                    PurchasePrice = vehicleEntity.PurchasePrice,
                    MakeId = vehicleEntity.Make.Id,
                    Images = vehicleEntity.Images.Select(img => img.FileName + img.MimeType).ToList(),
                    Status = vehicleEntity.Status.Status,
                    SellingPrice = vehicleEntity.Status.SellingPrice
                };
        }

        public async Task<VehicleViewModel> AddVehicle(VehicleInputViewModel vehicle, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding new vehicle {@Vehicle}", vehicle);
            var make = await _dbContext.VehicleMakes.FirstOrDefaultAsync(m => m.Id == vehicle.MakeId, cancellationToken);
            if (make == null)
            {
                throw new Exception("Invalid vehicle make selected");
            }
            var vehicleEntity = new Vehicle
            {
                Make = make,
                Trim = vehicle.Trim,
                Model = vehicle.Model,
                Year = vehicle.Year,
                PurchaseDate = vehicle.PurchaseDate,
                PurchasePrice = vehicle.PurchasePrice,
                Status = new()
                {
                    Status = LotDisplayStatus.Hidden,
                    DateAdded = DateTime.UtcNow,
                    SellDate = null,
                    SellingPrice = vehicle.PurchasePrice + 500,
                }
            };

            _dbContext.Vehicles.Add(vehicleEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Vehicle successfully added");
            return new VehicleViewModel
            {
                Id = vehicleEntity.Id,
                Make = vehicleEntity.Make.Name,
                Model = vehicleEntity.Model,
                Trim = vehicleEntity.Trim,
                Year = vehicleEntity.Year,
                PurchaseDate = vehicleEntity.PurchaseDate,
                PurchasePrice = vehicleEntity.PurchasePrice,
                MakeId = vehicleEntity.Make.Id,
            };
        }
       
        public async Task<VehicleViewModel> EditVehicle(int vehicleId, VehicleInputViewModel vehicle, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding vehicle with Id {VehicleId}", vehicleId);
            var vehicleEntity = await _dbContext.Vehicles
                                                    .Include(v => v.Status)
                                                    .Include(v => v.Make)
                                                    .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

            _logger.LogInformation("Vehicle lookup complete. Result: {@VehicleEntity}", vehicleEntity);

            if (vehicleEntity == null) throw new VehicleNotFoundException { VehicleId = vehicleId };

            if (vehicleEntity.MakeId != vehicle.MakeId)
            {
                _logger.LogInformation("Loading vehicle make {MakeId}", vehicle.MakeId);
                var @newMake = await _dbContext.VehicleMakes.FirstOrDefaultAsync(m => m.Id == vehicle.MakeId, cancellationToken);

                _logger.LogInformation("Make lookup complete. Result: {@NewMake}", @newMake);
                vehicleEntity.Make = newMake ?? throw new VehicleMakeNotFoundException { MakeId = vehicle.MakeId };
            }

            vehicleEntity.Year = vehicle.Year;
            vehicleEntity.Trim = vehicle.Trim;
            vehicleEntity.Model = vehicle.Model;
            vehicleEntity.PurchaseDate = vehicle.PurchaseDate;
            vehicleEntity.PurchasePrice = vehicle.PurchasePrice;
            _logger.LogInformation("Updating details to {@VehicleEntity}", vehicleEntity);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Update completed successfully");

            return new VehicleViewModel
            {
                Id = vehicleEntity.Id,
                Make = vehicleEntity.Make.Name,
                Model = vehicleEntity.Model,
                Trim = vehicleEntity.Trim,
                Year = vehicleEntity.Year,
                PurchaseDate = vehicleEntity.PurchaseDate,
                PurchasePrice = vehicleEntity.PurchasePrice,
                MakeId = vehicleEntity.Make.Id,
            };
        }
        
        public async Task<VehicleViewModel> RemoveVehicle(int vehicleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding vehicle with Id {VehicleId}", vehicleId);
            var vehicleEntity = await _dbContext.Vehicles
                                                    .Include(v => v.Status)
                                                    .Include(v => v.Make)
                                                    .Include(v => v.Images)
                                                    .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

            _logger.LogInformation("Vehicle lookup complete. Result: {@VehicleEntity}", vehicleEntity);

            if (vehicleEntity == null) throw new VehicleNotFoundException { VehicleId = vehicleId };
            var repairs = await _dbContext.Repairs.Where(v => v.VehicleId == vehicleEntity.Id).ToListAsync(cancellationToken);

            _dbContext.Vehicles.Remove(vehicleEntity);
            _dbContext.Remove(vehicleEntity.Status);
            _dbContext.Remove(vehicleEntity.Images);
            _dbContext.Remove(repairs);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Delete operation completed successfully");

            return new VehicleViewModel
            {
                Id = vehicleEntity.Id,
                Make = vehicleEntity.Make.Name,
                Model = vehicleEntity.Model,
                Trim = vehicleEntity.Trim,
                Year = vehicleEntity.Year,
                PurchaseDate = vehicleEntity.PurchaseDate,
                PurchasePrice = vehicleEntity.PurchasePrice,
                MakeId = vehicleEntity.Make.Id,
            };
        }
    }
}
