using CurConvApp.ViewModels;
using FluentValidation;

namespace CurConvApp.Validators
{
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Surname)
                .NotEmpty()
                .WithMessage("Surname is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Valid email is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters.");
        }
    }
}
