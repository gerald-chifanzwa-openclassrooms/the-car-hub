using CarHub.Models;
using FluentValidation;

namespace CarHub.Validators
{
    public class VehicleRepairInputViewModelValidator : AbstractValidator<VehicleRepairInputViewModel>
    {
        public VehicleRepairInputViewModelValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(100)
                .Matches(@"^\b((?!=|\,|\.).)+([^<>]+)\b$")
                .WithMessage("Invalid format for description");

            RuleFor(x => x.Cost)
                .NotEmpty()
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(99999);
        }
    }
}
