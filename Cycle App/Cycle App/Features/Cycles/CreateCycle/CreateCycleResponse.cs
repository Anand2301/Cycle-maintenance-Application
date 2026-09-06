namespace CycleMaintenance.Features.Cycles.CreateCycle;

public class CreateCycleResponse
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string Model { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }

    public Guid QrToken { get; set; }

    public DateTime CreatedAt { get; set; }
}