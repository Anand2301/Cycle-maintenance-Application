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
}
