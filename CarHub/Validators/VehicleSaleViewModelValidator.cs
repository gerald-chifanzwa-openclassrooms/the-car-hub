using CarHub.Models;
using FluentValidation;

namespace CarHub.Validators
{
    public class VehicleSaleViewModelValidator : AbstractValidator<VehicleSaleViewModel>
    {
        public VehicleSaleViewModelValidator()
        {
            RuleFor(m => m.SellingPrice)
                .NotEmpty()
                .GreaterThan(0)
                .Must((model, val) => val > (model.PurchasePrice * 0.75M))
                .WithMessage("Are you sure you are selling at more than 25% loss?");

            RuleFor(m => m.SellDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(model => model.PurchaseDate)
                .WithMessage("You couldn't possibly sell the car before the purchase date, could you?");
        }
    }
}
