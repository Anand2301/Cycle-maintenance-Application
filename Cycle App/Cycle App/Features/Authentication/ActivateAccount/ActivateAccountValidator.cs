using FastEndpoints;
using FluentValidation;

namespace CycleMaintenance.Features.Authentication.ActivateAccount;

public class ActivateAccountValidator
    : Validator<ActivateAccountRequest>
{
    public ActivateAccountValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Activation token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters.");
    }
}