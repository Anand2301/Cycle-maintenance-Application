namespace Cycle_App.Features.Tickets.UpdateTicket
{

    public class UpdateTicketRequest
    {
        public Guid TicketId { get; set; }

        public Guid? StatusId { get; set; }

        public Guid? PriorityId { get; set; }

        public Guid? AssigneeId { get; set; }

        public string? ContactPhone { get; set; }

        public string? ContactEmail { get; set; }

        public string? Description { get; set; }
    }
}
