using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Tickets.GetAssignedTickets;

public class GetAssignedTicketsEndpoint
    : EndpointWithoutRequest<List<GetAssignedTicketsEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ICurrentUserService _currentUser;

    public GetAssignedTicketsEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        ICurrentUserService currentUser)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _currentUser = currentUser;
    }

    public override void Configure()
    {
        Get("/api/tickets/assigned");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                t.Id,
                t.CustomerId,
                t.CycleId,
                c.Name AS CustomerName,
                cy.Model AS CycleModel,
                cy.SerialNumber,
                t.CompanyName,
                t.StatusId,
                s.Name AS Status,
                t.PriorityId,
                p.Name AS Priority,
                t.AssigneeId,
                CONCAT(u.FirstName, ' ', COALESCE(u.LastName, '')) AS AssigneeName,
                t.ContactPhone,
                t.ContactEmail,
                t.Description,
                t.CreatedAt,
                t.UDF1, 
                t.UDF2, 
                t.UDF3, 
                t.UDF4, 
                t.UDF5, 
                t.UDF6, 
                t.UDF7, 
                t.UDF8, 
                t.UDF9, 
                t.UDF10,
                t.UpdatedAt
            FROM Tickets t
            INNER JOIN Customers c
                ON t.CustomerId = c.Id
            INNER JOIN Cycles cy
                ON t.CycleId = cy.Id
            INNER JOIN Statuses s
                ON t.StatusId = s.Id
            INNER JOIN Priorities p
                ON t.PriorityId = p.Id
            LEFT JOIN Users u
                ON t.AssigneeId = u.Id
            WHERE t.AssigneeId = @UserId
            ORDER BY t.CreatedAt DESC;
            """;

        var tickets = await connection.QueryAsync<Response>(
            sql,
            new
            {
                UserId = _currentUser.UserId
            });

        await Send.OkAsync(tickets.ToList(), ct);
    }

    public class Response
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CycleId { get; set; }

        public string? CustomerName { get; set; }
        public string? CycleModel { get; set; }
        public string? SerialNumber { get; set; }

        public string? CompanyName { get; set; }

        public Guid StatusId { get; set; }
        public string? Status { get; set; }

        public Guid PriorityId { get; set; }
        public string? Priority { get; set; }

        public Guid? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }

        public string ContactPhone { get; set; } = string.Empty;
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

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}