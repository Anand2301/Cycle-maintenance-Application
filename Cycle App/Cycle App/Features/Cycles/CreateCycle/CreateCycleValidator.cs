using Cycle_App.Features.Cycles.CreateCycle;
using FastEndpoints;
using FluentValidation;

namespace CycleMaintenance.Features.Cycles.CreateCycle;

public class CreateCycleValidator : Validator<CreateCycleRequest>
{
    public CreateCycleValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer is required.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Cycle model is required.")
            .MaximumLength(150)
            .WithMessage("Cycle model cannot exceed 150 characters.");

        RuleFor(x => x.SerialNumber)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
            .WithMessage("Serial number cannot exceed 100 characters.");
    }
}