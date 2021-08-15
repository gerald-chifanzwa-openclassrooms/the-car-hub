using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Data;
using CarHub.Models;

namespace CarHub.Services
{
    public interface IVehicleMakeService
    {
        Task<VehicleMakeViewModel> FindMake(int makeId, CancellationToken cancellationToken);
        Task<IEnumerable<VehicleMakeViewModel>> GetVehicleMakes(CancellationToken cancellationToken);
        Task<VehicleMakeViewModel> AddMake(VehicleMakeViewModel model, CancellationToken cancellationToken);
    }
}
