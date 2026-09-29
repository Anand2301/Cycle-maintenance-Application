namespace CycleMaintenance.Features.Tickets.GetTicket;

public class GetTicketResponse
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid CycleId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string CycleModel { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }

    public string? CompanyName { get; set; }

    public Guid StatusId { get; set; }

    public string Status { get; set; } = string.Empty;

    public Guid PriorityId { get; set; }

    public string Priority { get; set; } = string.Empty;

    public Guid? AssigneeId { get; set; }

    public string? AssigneeName { get; set; }

    public string ContactPhone { get; set; } = string.Empty;

    public string? ContactEmail { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UDF1 { get; set; }
    public string? UDF2 { get; set; }
    public string? UDF3 { get; set; }
    public string? UDF4 { get; set; }
    public string? UDF5 { get; set; }
    public string? UDF6 { get; set; }
    public string? UDF7 { get; set; }
    public string? UDF8 { get; set; }
    public string? UDF9 { get; set; }
    public string? UDF10 { get; set; }
}