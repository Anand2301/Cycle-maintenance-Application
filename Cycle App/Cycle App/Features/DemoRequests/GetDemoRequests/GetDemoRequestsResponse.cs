namespace Cycle_App.Features.DemoRequests.GetDemoRequests;

public class GetDemoRequestsResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public Guid? ServiceCenterId { get; set; }

    public Guid? StatusId { get; set; }

    public DateTime CreatedAt { get; set; }
}
