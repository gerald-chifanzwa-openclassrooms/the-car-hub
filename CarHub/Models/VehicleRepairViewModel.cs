using System;
using System.ComponentModel.DataAnnotations;

namespace CarHub.Models
{
    public class VehicleRepairViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:$#,##0.00}")]
        public decimal Cost { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime RepairDate { get; set; }
        public int VehicleId { get; set; }
    }
}
