

namespace CarHub.Models
{
    public class PublishVehicleViewModel : VehicleViewModel
    {
        public PublishVehicleViewModel() { }
        public PublishVehicleViewModel(VehicleViewModel model)
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
    }
}
