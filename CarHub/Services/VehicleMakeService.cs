using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Data;
using CarHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarHub.Services
{
    public class VehicleMakeService : IVehicleMakeService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<VehicleMakeService> _logger;

        public VehicleMakeService(ApplicationDbContext dbContext, ILogger<VehicleMakeService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<VehicleMakeViewModel> FindMake(int makeId, CancellationToken cancellationToken)
        {
            return await _dbContext.VehicleMakes
                .Where(m => m.Id == makeId)
                .Select(m => new VehicleMakeViewModel
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<VehicleMakeViewModel> AddMake(VehicleMakeViewModel model, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding vehicle make {@Model}", model);
            var entity = new VehicleMake { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.Name) };
            _dbContext.VehicleMakes.Add(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Vehicle Make added");
            return new VehicleMakeViewModel
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
        public async Task<IEnumerable<VehicleMakeViewModel>> GetVehicleMakes(CancellationToken cancellationToken)
        {
            return await _dbContext.VehicleMakes.Select(m => new VehicleMakeViewModel
            {
                Id = m.Id,
                Name = m.Name
            }).ToListAsync(cancellationToken);
        }
    }
}
