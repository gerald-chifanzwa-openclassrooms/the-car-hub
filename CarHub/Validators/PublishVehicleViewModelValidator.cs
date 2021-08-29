using CarHub.Models;
using FluentValidation;

namespace CarHub.Validators
{
    public class PublishVehicleViewModelValidator : AbstractValidator<PublishVehicleViewModel>
    {
        public PublishVehicleViewModelValidator()
        {
            RuleFor(x => x.SellingPrice)
                .NotEmpty()
                .GreaterThanOrEqualTo(model => model.PurchasePrice)
                .WithMessage("You can't sell a vehicle for less that what you paid for it");
        }
    }
}
