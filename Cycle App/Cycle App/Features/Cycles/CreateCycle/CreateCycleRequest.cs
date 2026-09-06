namespace Cycle_App.Features.Cycles.CreateCycle;

public class CreateCycleRequest
{
    public Guid CustomerId { get; set; }

    public string Model { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }
}
