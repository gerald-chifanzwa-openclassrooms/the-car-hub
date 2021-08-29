using System;
using CarHub.Models;
using FluentValidation;

namespace CarHub.Validators
{
    public class VehicleImageUploadViewModelValidator : AbstractValidator<VehicleImageUploadViewModel>
    {
        public VehicleImageUploadViewModelValidator()
        {
            RuleFor(f => f.File)
                .Must(file =>
                {
                    var type = file.ContentType;
                    return type.StartsWith("Image/", StringComparison.InvariantCultureIgnoreCase);
                })
                .WithMessage("Only image files are allowed");
        }
    }
}
