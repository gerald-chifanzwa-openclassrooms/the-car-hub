using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarHub.Models;
using CarHub.Services;
using FluentValidation;

namespace CarHub.Validators
{
    public class VehicleInputViewModelValidator : AbstractValidator<VehicleInputViewModel>
    {
        public VehicleInputViewModelValidator(IVehicleMakeService makeService)
        {
            RuleFor(v => v.Year)
                .NotEmpty()
                .GreaterThanOrEqualTo(1990)
                .WithMessage("Vehicles older than 1990 are not allowed")
                .LessThanOrEqualTo(2021)
                .WithMessage("You may have captured a future year there");

            RuleFor(v => v.MakeId)
                .NotEmpty()
                .MustAsync(async (modelValue, cancellationToken) =>
                {
                    var make = await makeService.FindMake(modelValue, cancellationToken);
                    return make is not null;
                })
                .WithMessage("Please select a valid make");

            RuleFor(v => v.Model)
                .NotEmpty()
                .Matches(@"^[a-zA-Z0-9]+([\s\-]?[a-zA-Z0-9]+)*$")
                .WithMessage("Please enter the a correct Model type")
                .MaximumLength(50)
                .WithMessage("Model name should not exceed {MaxLength} characters");


            RuleFor(v => v.Trim)
                .NotEmpty()
                .Matches(@"^[a-zA-Z0-9]+([\s\-]?[a-zA-Z0-9]+)*$")
                .WithMessage("Please enter the a correct Trim")
                .MaximumLength(50)
                .WithMessage("Trim name should not exceed {MaxLength} characters");

            RuleFor(v => v.PurchasePrice)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Purchase price should be greater than $0.00")
                .LessThanOrEqualTo(99999)
                .WithMessage("Purchases above {Max} are not allowed");

            RuleFor(v => v.PurchaseDate)
                .NotEmpty()
                .Must((value) => value <= DateTime.UtcNow && value > new DateTime(2020, 1, 1))
                .WithMessage("Vehicles must have been captured within the last few months");
        }
    }
}
