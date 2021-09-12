

using System;

namespace CarHub.Models
{
    public class VehicleSaleViewModel : VehicleViewModel
    {
        public VehicleSaleViewModel() { }
        public VehicleSaleViewModel(VehicleViewModel model)
        {
            Id = model.Id;
            Make = model.Make;
            PurchaseDate = model.PurchaseDate;
            PurchasePrice = model.PurchasePrice;
            Model = model.Model;
            Trim = model.Trim;
            SellingPrice = model.SellingPrice;
            Status = model.Status;
            Year = model.Year;
        }

        public DateTime SellDate { get; set; } = DateTime.Today;
    }
}
