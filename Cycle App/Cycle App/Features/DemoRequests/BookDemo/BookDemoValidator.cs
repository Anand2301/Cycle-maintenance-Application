using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using FluentValidation;

namespace Cycle_App.Features.DemoRequests.BookDemo;

public class BookDemoValidator : Validator<BookDemoRequest>
{
    public BookDemoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is Required")
            .MaximumLength(200)
            .WithMessage("Name Cannot exceed 200 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("PhoneNumber is Required")
            .MaximumLength(20)
            .WithMessage("Pone number cannot exceed 20 numbers");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Please provide a valid email address");

        RuleFor(x => x.Address)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address cannot be exceed 1000 characters");

    }
}
