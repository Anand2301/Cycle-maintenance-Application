namespace Cycle_App.Features.Cycles.GetCycleServiceInfo;

public class GetCycleServiceInfoResponse
{
    public Guid CycleId { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }

    public CustomerInfo Customer { get; set; } = new();

    public ServiceInfo? CurrentService { get; set; }

    public List<ServiceInfo> PreviousServices { get; set; } = [];
}

public class CustomerInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public class ServiceInfo
{
    public Guid TicketId { get; set; }
    public Guid StatusId { get; set; }
    public string? Status { get; set; }

    public Guid PriorityId { get; set; }
    public string? Priority { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}