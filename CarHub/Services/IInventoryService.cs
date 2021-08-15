﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;

namespace CarHub.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<VehicleViewModel>> GetAllVehiclesAsync(CancellationToken cancellationToken);
        Task<VehicleDetailsViewModel> GetVehicleAsync(int vehicleId, CancellationToken cancellationToken);
        Task<VehicleViewModel> AddVehicle(VehicleInputViewModel vehicle, CancellationToken cancellationToken);
        Task<VehicleViewModel> RemoveVehicle(int vehicleId, CancellationToken cancellationToken);
        Task<VehicleViewModel> EditVehicle(int vehicleId, VehicleInputViewModel vehicle, CancellationToken cancellationToken);
    }
}