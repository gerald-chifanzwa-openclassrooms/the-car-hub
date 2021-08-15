using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarHub.Models
{
    public class VehicleInputViewModel
    {
        [BindRequired, Display(Name = "Vehicle Make")]
        public int MakeId { get; set; }
        [Required, MaxLength(50)]
        public string Model { get; set; }
        [Required, MaxLength(50)]
        public string Trim { get; set; }
        [Required, BindRequired, Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }
        [BindRequired, Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [BindRequired]
        public int Year { get; set; }
    }
}
