using Cycle_App.Features.Customers.CreateCustomer;
using FastEndpoints;
using FluentValidation;

namespace CycleMaintenance.Features.Customers.CreateCustomer;

public class CreateCustomerValidator : Validator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Please provide a valid email address.");

        RuleFor(x => x.Address)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address cannot exceed 1000 characters.");
    }
}