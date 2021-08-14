using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;

namespace CarHub.Services
{
    public interface IVehicleMakeService
    {
        public Task<IEnumerable<VehicleMakeViewModel>> GetVehicleMakes(CancellationToken cancellationToken);
        public Task<VehicleMakeViewModel> AddMake(VehicleMakeViewModel model, CancellationToken cancellationToken);
    }
}
