using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;

namespace CarHub.Services
{
    public interface IVehicleRepairsService
    {
        Task<VehicleRepairViewModel> AddRepair(int vehicleId, VehicleRepairInputViewModel model, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<VehicleRepairViewModel>> GetVehicleRepairs(int vehicleId, CancellationToken cancellationToken);
    }
}
