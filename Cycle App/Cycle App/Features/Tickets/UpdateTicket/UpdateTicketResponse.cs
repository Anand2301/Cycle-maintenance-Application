namespace Cycle_App.Features.Tickets.UpdateTicket;


public class UpdateTicketResponse
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid CycleId { get; set; }

    public string? CompanyName { get; set; }

    public Guid StatusId { get; set; }

    public Guid PriorityId { get; set; }

    public Guid? AssigneeId { get; set; }

    public string ContactPhone { get; set; } = string.Empty;

    public string? ContactEmail { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UpdatedBy { get; set; }
}
