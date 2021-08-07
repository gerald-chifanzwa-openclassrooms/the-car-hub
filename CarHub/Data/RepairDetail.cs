using System;

namespace CarHub.Data
{
    public class RepairDetail
    {
        public int Id { get; set; }
        public Vehicle Vehicle { get; set; }
        public int VehicleId { get; set; }
        public DateTime Date { get; set; }
        public int Description { get; set; }
        public decimal Cost { get; set; }
    }
}
