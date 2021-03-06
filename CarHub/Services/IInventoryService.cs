using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;
using Microsoft.AspNetCore.Http;

namespace CarHub.Services
{
    public interface IInventoryService
    {
        Task<PagedResultSet<VehicleDetailsViewModel>> GetAllVehiclesAsync(GetVehiclesRequest request, CancellationToken cancellationToken);
        Task<VehicleDetailsViewModel> GetVehicleAsync(int vehicleId, CancellationToken cancellationToken);
        Task<VehicleViewModel> AddVehicle(VehicleInputViewModel vehicle, CancellationToken cancellationToken);
        Task<VehicleViewModel> RemoveVehicle(int vehicleId, CancellationToken cancellationToken);
        Task<VehicleViewModel> EditVehicle(int vehicleId, VehicleInputViewModel vehicle, CancellationToken cancellationToken);
        Task AddVehicleImage(int vehicleId, IFormFile imageFile, CancellationToken cancellationToken);
        Task<VehicleViewModel> PublishVehicle(int vehicleId, PublishVehicleViewModel vehicle, CancellationToken cancellationToken);
        Task<VehicleViewModel> FlagSoldVehicle(int vehicleId, VehicleSaleViewModel vehicle, CancellationToken cancellationToken);
    }
}
