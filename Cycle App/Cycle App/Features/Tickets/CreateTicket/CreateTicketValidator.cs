using FastEndpoints;
using FluentValidation;

namespace Cycle_App.Features.Tickets.CreateTicket;

public class CreateTicketValidator : Validator<CreateTicketRequest>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer is required.");

        RuleFor(x => x.CycleId)
            .NotEmpty()
            .WithMessage("Cycle is required.");

        RuleFor(x => x.CompanyName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.CompanyName))
            .WithMessage("Company name cannot exceed 200 characters.");

        RuleFor(x => x.StatusId)
            .NotEmpty()
            .WithMessage("Status is required.");

        RuleFor(x => x.PriorityId)
            .NotEmpty()
            .WithMessage("Priority is required.");

        RuleFor(x => x.ContactPhone)
            .NotEmpty()
            .WithMessage("Contact phone is required.")
            .MaximumLength(20)
            .WithMessage("Contact phone cannot exceed 20 characters.");

        RuleFor(x => x.ContactEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
            .WithMessage("Please provide a valid email address.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 2000 characters.");
    }
}