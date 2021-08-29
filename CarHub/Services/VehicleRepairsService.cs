using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Data;
using CarHub.Exceptions;
using CarHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarHub.Services
{
    public class VehicleRepairsService : IVehicleRepairsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<VehicleRepairsService> _logger;

        public VehicleRepairsService(ApplicationDbContext dbContext, ILogger<VehicleRepairsService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<VehicleRepairViewModel> AddRepair(int vehicleId, VehicleRepairInputViewModel model, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding vehicle repair {@Model}", model);

            var vehicle = await _dbContext.Vehicles
                                                .Include(v => v.Status)
                                                .Include(v => v.Repairs)
                                                .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

            if (vehicle == null) throw new VehicleNotFoundException { VehicleId = vehicleId };

            var totalRepairsCost = vehicle.Repairs.Sum(r => r.Cost) + model.Cost;
            vehicle.Status.SellingPrice = vehicle.PurchasePrice + totalRepairsCost + 500;

            var repair = new RepairDetail
            {
                Vehicle = vehicle,
                Cost = model.Cost,
                Date = model.RepairDate,
                Description = model.Description,
            };

            _dbContext.Repairs.Add(repair);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Repair successfully saved");

            return new VehicleRepairViewModel
            {
                VehicleId = repair.VehicleId,
                RepairDate = repair.Date,
                Description = repair.Description,
                Cost = repair.Cost,
                Id = repair.Id,
            };
        }

        public async Task<IReadOnlyCollection<VehicleRepairViewModel>> GetVehicleRepairs(int vehicleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving repairs for vehicle {VehicleId}", vehicleId);
            var repairs = await _dbContext.Repairs.Where(r => r.VehicleId == vehicleId)
                .Select(repair => new VehicleRepairViewModel
                {
                    VehicleId = repair.VehicleId,
                    RepairDate = repair.Date,
                    Description = repair.Description,
                    Cost = repair.Cost,
                    Id = repair.Id,
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} repairs", repairs.Count);

            return repairs.AsReadOnly();
        }
    }
}
