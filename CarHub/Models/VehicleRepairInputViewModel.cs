using System;
using System.ComponentModel.DataAnnotations;

namespace CarHub.Models
{
    public class VehicleRepairInputViewModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Cost { get; set; }
        [Display(Name = "Repair Date")]
        public DateTime RepairDate { get; set; }
    }
}
