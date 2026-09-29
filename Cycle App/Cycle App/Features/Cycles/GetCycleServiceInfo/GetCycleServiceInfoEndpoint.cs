using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Cycles.GetCycleServiceInfo;

public class GetCycleServiceInfoEndpoint
    : Endpoint<GetCycleServiceInfoEndpoint.Request, GetCycleServiceInfoResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ICurrentUserService _currentUser;

    public GetCycleServiceInfoEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        ICurrentUserService currentUser)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _currentUser = currentUser;
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

        // 1. Get cycle and customer information.
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

        // 2. Start with basic cycle information.
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
            }
        };

        // 3. Customer only gets cycle information.
        if (string.Equals(
                _currentUser.Role,
                "Customer",
                StringComparison.OrdinalIgnoreCase))
        {
            await Send.OkAsync(response, ct);
            return;
        }

        // 4. Employee and Admin need the latest service request.
        const string currentServiceSql = """
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
            AND t.StatusId IN
            (
                '056cddda-c5fe-423d-82ef-8283b00ecaea', -- New
                'b873703b-0c37-4f01-9016-d254b826836b', -- Assigned
                '5e865e00-dd28-4d64-b535-981fe310e0cf'  -- In Progress
            )
            ORDER BY t.CreatedAt DESC
            LIMIT 1;
            """;

        response.CurrentService =
            await connection.QuerySingleOrDefaultAsync<ServiceInfo>(
                currentServiceSql,
                new
                {
                    req.CycleId
                });

        // 5. Employee only gets current service.
        if (string.Equals(
                _currentUser.Role,
                "Employee",
                StringComparison.OrdinalIgnoreCase))
        {
            await Send.OkAsync(response, ct);
            return;
        }

        // 6. Admin gets the complete service history.
        if (string.Equals(
                _currentUser.Role,
                "Admin",
                StringComparison.OrdinalIgnoreCase))
        {
            const string historySql = """
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
                ORDER BY t.CreatedAt DESC
                OFFSET 1;
                """;

            response.PreviousServices = (
                await connection.QueryAsync<ServiceInfo>(
                    historySql,
                    new
                    {
                        req.CycleId
                    })
            ).ToList();
        }

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