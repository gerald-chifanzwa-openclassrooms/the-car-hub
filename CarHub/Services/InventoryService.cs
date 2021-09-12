using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(ApplicationDbContext dbContext, ILogger<InventoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<PagedResultSet<VehicleDetailsViewModel>> GetAllVehiclesAsync(GetVehiclesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Vehicle> query = _dbContext.Vehicles
                                           .Include(v => v.Status)
                                           .Include(v => v.Make)
                                           .Include(v => v.Images);
            Expression<Func<Vehicle, bool>> filterExpression = request switch
            {
                null => (v) => v.Status.Status == LotDisplayStatus.Show,
                { IsUserAuthenticated: true, Status: null } => (v) => v.Status.Status != LotDisplayStatus.Sold,
                { IsUserAuthenticated: true, Status: var x } when x != null => v => v.Status.Status == request.Status,
                { IsUserAuthenticated: false, Make: var m } when m != null => (v) => v.Status.Status == LotDisplayStatus.Show && v.MakeId == m,
                _ => (v) => v.Status.Status == LotDisplayStatus.Show,
            };

            query = query.Where(filterExpression);
            var totalCount = await query.CountAsync(cancellationToken);
            var vehicles = await query.Select(v => new VehicleDetailsViewModel
            {
                Id = v.Id,
                Make = v.Make.Name,
                Model = v.Model,
                Trim = v.Trim,
                Year = v.Year,
                PurchaseDate = v.PurchaseDate,
                PurchasePrice = v.PurchasePrice,
                MakeId = v.Make.Id,
                SellingPrice = v.Status.SellingPrice,
                Status = v.Status.Status,
                Images = v.Images.Select(img => $"/images/{v.Id}/{img.FileName}").ToList()
            }).OrderBy(v => v.PurchaseDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultSet<VehicleDetailsViewModel>
            {
                Items = vehicles,
                TotalCount = totalCount,
                CurrentPage = request.PageNumber,
            };
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
                    Images = vehicleEntity.Images.Select(img => $"/images/{vehicleEntity.Id}/{img.FileName}").ToList(),
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

        public async Task AddVehicleImage(int vehicleId, IFormFile imageFile, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream, cancellationToken);
            var extension = Path.GetExtension(imageFile.FileName);
            var mimeType = imageFile.ContentType;

            var name = Path.ChangeExtension($"{vehicleId}-{DateTime.Now.Ticks}", extension);

            // Upload the file if less than 2 MB
            if (memoryStream.Length >= 2097152)
                throw new FileTooLargeException();

            _logger.LogInformation("Uploading image file {FileName}: {Size:#.00kB}", imageFile.FileName, memoryStream.Length / 1024M);
            var file = new VehicleImage()
            {
                ImageData = memoryStream.ToArray(),
                FileName = name,
                VehicleId = vehicleId,
                MimeType = mimeType,
            };
            _dbContext.VehicleImages.Add(file);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Upload successfull");
        }

        public async Task<VehicleViewModel> PublishVehicle(int vehicleId, PublishVehicleViewModel vehicle, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding vehicle with Id {VehicleId} for publish", vehicleId);
            var vehicleEntity = await _dbContext.Vehicles
                                                    .Include(v => v.Status)
                                                    .Include(v => v.Make)
                                                    .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

            _logger.LogInformation("Vehicle lookup complete. Result: {@VehicleEntity}", vehicleEntity);

            if (vehicleEntity == null) throw new VehicleNotFoundException { VehicleId = vehicleId };

            vehicleEntity.Status.SellingPrice = vehicle.SellingPrice;
            vehicleEntity.Status.Status = LotDisplayStatus.Show;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Vehicle {VehicleId} successfully published at ${SellingPrice:#,##0.00}", vehicleId, vehicle.SellingPrice);

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

        public async Task<VehicleViewModel> FlagSoldVehicle(int vehicleId, VehicleSaleViewModel vehicle, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding vehicle with Id {VehicleId} for publish", vehicleId);
            var vehicleEntity = await _dbContext.Vehicles
                                                    .Include(v => v.Status)
                                                    .Include(v => v.Make)
                                                    .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

            _logger.LogInformation("Vehicle lookup complete. Result: {@VehicleEntity}", vehicleEntity);

            if (vehicleEntity == null) throw new VehicleNotFoundException { VehicleId = vehicleId };

            vehicleEntity.Status.SellingPrice = vehicle.SellingPrice;
            vehicleEntity.Status.Status = LotDisplayStatus.Sold;
            vehicleEntity.Status.SellDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Vehicle {VehicleId} successfully flagged as sold at ${SellingPrice:#,##0.00}", vehicleId, vehicle.SellingPrice);

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
