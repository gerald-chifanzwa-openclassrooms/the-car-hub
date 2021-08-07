using System;
using System.Collections.Generic;

namespace CarHub.Data
{
    public class LotStatus
    {
        public int Id { get; set; }
        public Vehicle Vehicle { get; set; }
        public int VehicleId { get; set; }
        public DateTime DateAdded { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime? SellDate { get; set; }
    }
}
