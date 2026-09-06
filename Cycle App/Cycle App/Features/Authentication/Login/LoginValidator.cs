using FastEndpoints;
using FluentValidation;

namespace CycleMaintenance.Features.Authentication.Login;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage("Email is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}