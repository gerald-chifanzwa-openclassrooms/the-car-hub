using System.Collections.Generic;

namespace CarHub.Models
{
    public class VehicleDetailsViewModel : VehicleViewModel
    {
        public ICollection<string> Images { get; set; }
    }
}
