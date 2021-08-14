using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using CarHub.Data;

namespace CarHub.Models
{
    public class VehicleViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime PurchaseDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:$#,##0.00}")]
        public decimal PurchasePrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:$#,##0.00}")]
        public decimal SellingPrice { get; set; }
        public int Id { get; set; }
        public int MakeId { get; set; }
        [DisplayFormat(DataFormatString = "{0:D4}")]
        public int Year { get; set; }
        public LotDisplayStatus Status { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Trim { get; set; }
    }
}
