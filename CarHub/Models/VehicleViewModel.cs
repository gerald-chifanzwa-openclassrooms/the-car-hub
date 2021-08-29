using System;
using System.ComponentModel.DataAnnotations;
using CarHub.Data;

namespace CarHub.Models
{
    public class VehicleViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:$#,##0.00}")]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:$#,##.00}")]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }
        public int Id { get; set; }
        [Display(Name = "Make")]
        public int MakeId { get; set; }
        [DisplayFormat(DataFormatString = "{0:D4}")]
        [Display(Name = "Vehicle Year")]
        public int Year { get; set; }
        public LotDisplayStatus Status { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Trim { get; set; }
    }
}
