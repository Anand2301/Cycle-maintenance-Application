using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Cycles.GetCycleServiceInfo;

public class GetCycleServiceInfoEndpoint
    : Endpoint<GetCycleServiceInfoEndpoint.Request, GetCycleServiceInfoResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCycleServiceInfoEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/cycles/{CycleId}/service-info");
    }

    public override async Task HandleAsync(
        Request req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string cycleSql = """
            SELECT
                cy.Id,
                cy.Model,
                cy.SerialNumber,
                c.Id AS CustomerId,
                c.Name AS CustomerName,
                c.PhoneNumber AS CustomerPhoneNumber,
                c.Email AS CustomerEmail
            FROM Cycles cy
            INNER JOIN Customers c
                ON cy.CustomerId = c.Id
            WHERE cy.Id = @CycleId;
            """;

        var cycle = await connection.QuerySingleOrDefaultAsync<CycleInfo>(
            cycleSql,
            new
            {
                req.CycleId
            });

        if (cycle is null)
        {
            AddError(
                "CycleId",
                "Cycle not found.");

            await Send.ErrorsAsync(404, ct);
            return;
        }

        const string ticketSql = """
            SELECT
                t.Id AS TicketId,
                t.StatusId,
                s.Name AS Status,
                t.PriorityId,
                p.Name AS Priority,
                t.Description,
                t.CreatedAt,
                t.UpdatedAt
            FROM Tickets t
            INNER JOIN Statuses s
                ON t.StatusId = s.Id
            INNER JOIN Priorities p
                ON t.PriorityId = p.Id
            WHERE t.CycleId = @CycleId
            ORDER BY t.CreatedAt DESC;
            """;

        var services = (
            await connection.QueryAsync<ServiceInfo>(
                ticketSql,
                new
                {
                    req.CycleId
                })
        ).ToList();

        var response = new GetCycleServiceInfoResponse
        {
            CycleId = cycle.Id,
            Model = cycle.Model,
            SerialNumber = cycle.SerialNumber,

            Customer = new CustomerInfo
            {
                Id = cycle.CustomerId,
                Name = cycle.CustomerName,
                PhoneNumber = cycle.CustomerPhoneNumber,
                Email = cycle.CustomerEmail
            },

            CurrentService = services.FirstOrDefault(),

            PreviousServices = services
                .Skip(1)
                .ToList()
        };

        await Send.OkAsync(response, ct);
    }

    public class Request
    {
        public Guid CycleId { get; set; }
    }

    private class CycleInfo
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerEmail { get; set; }
    }
}