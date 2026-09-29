using Cycle_App.Data;
using CycleMaintenance.Features.Tickets.GetTicket;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Tickets.GetTicket;

public class GetTicketEndpoint
    : Endpoint<GetTicketEndpoint.Request, GetTicketResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetTicketEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/tickets/{TicketId}");
    }

    public override async Task HandleAsync(
        Request req,
        CancellationToken ct)
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
                CONCAT(
                    u.FirstName,
                    ' ',
                    COALESCE(u.LastName, '')
                ) AS AssigneeName,

                t.ContactPhone,
                t.ContactEmail,
                t.Description,

                t.CreatedAt,
                t.CreatedBy,

                t.UpdatedAt,
                t.UpdatedBy,
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

                CONCAT(
                    ub.FirstName,
                    ' ',
                    COALESCE(ub.LastName, '')
                ) AS UpdatedByName

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

            LEFT JOIN Users ub
                ON t.UpdatedBy = ub.Id

            WHERE t.Id = @TicketId;
            """;

        var ticket = await connection.QuerySingleOrDefaultAsync<GetTicketResponse>(
            sql,
            new
            {
                req.TicketId
            });

        if (ticket is null)
        {
            AddError(
                "TicketId",
                "Ticket not found.");

            await Send.ErrorsAsync(404, ct);
            return;
        }

        await Send.OkAsync(ticket, ct);
    }

    public class Request
    {
        public Guid TicketId { get; set; }
    }
}