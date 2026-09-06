using Cycle_App.Data;
using CycleMaintenance.Features.Tickets.GetTicket;
using Dapper;
using FastEndpoints;

namespace CycleMaintenance.Features.Tickets.GetTickets;

public class GetTicketsEndpoint
    : EndpointWithoutRequest<List<GetTicketResponse>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetTicketsEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/tickets");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                t.Id,
                c.Name AS CustomerName,
                cy.Model AS CycleModel,
                cy.SerialNumber,
                s.Name AS Status,
                p.Name AS Priority,
                CONCAT(u.FirstName, ' ', COALESCE(u.LastName, '')) AS AssigneeName,
                t.ContactPhone,
                t.ContactEmail,
                t.Description,
                t.CreatedAt
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
            ORDER BY t.CreatedAt DESC;
            """;

        var tickets = await connection.QueryAsync<GetTicketResponse>(sql);

        await Send.OkAsync(
            tickets.ToList(),
            ct);
    }
}