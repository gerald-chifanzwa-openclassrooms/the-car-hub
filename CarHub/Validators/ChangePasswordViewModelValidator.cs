using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarHub.Models;
using FluentValidation;

namespace CarHub.Validators
{
    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordViewModelValidator()
        {
            RuleFor(m => m.OldPassword)
                .NotEmpty();

            RuleFor(m => m.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .DependentRules(() =>
                {

                    RuleFor(m => m.ConfirmPassword)
                        .NotEmpty()
                        .Must((model, value) => value == model.NewPassword)
                        .WithMessage("Must match your new password");
                });
        }
    }
}
