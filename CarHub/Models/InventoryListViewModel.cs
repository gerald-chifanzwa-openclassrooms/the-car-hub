using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarHub.Data;

namespace CarHub.Models
{
    public class InventoryListViewModel
    {
        public IEnumerable<VehicleDetailsViewModel> Vehicles { get; set; }
        public int CurrentPage { get; set; }
        public int TotalVehicles { get; set; }
        public int PageSize { get; set; }
        public LotDisplayStatus? StatusFilter { get; set; }
    }
}
