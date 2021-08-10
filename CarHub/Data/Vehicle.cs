using System;
using System.Collections.Generic;

namespace CarHub.Data
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int MakeId { get; set; }
        public VehicleMake Make { get; set; }
        public string Model { get; set; }
        public string Trim { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public LotStatus Status { get; set; }
        public ICollection<VehicleImage> Images { get; set; } = new List<VehicleImage>();
    }
}
