using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Dashboard.GetTickets;

public class GetDashboardTicketsEndpoint
    : EndpointWithoutRequest<List<GetDashboardTicketsEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardTicketsEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        ICurrentUserService currentUser)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _currentUser = currentUser;
    }

    public override void Configure()
    {
        Get("/api/dashboard/tickets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        // Admin → all recent tickets
        // Employee → only tickets assigned to the logged-in user

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
            WHERE
                @IsAdmin = TRUE
                OR (
                    @IsEmployee = TRUE
                    AND t.AssigneeId = @UserId
                )
            ORDER BY t.CreatedAt DESC
            LIMIT 10;
            """;

        var isAdmin = string.Equals(
            _currentUser.Role,
            "Admin",
            StringComparison.OrdinalIgnoreCase);

        var isEmployee = string.Equals(
            _currentUser.Role,
            "Employee",
            StringComparison.OrdinalIgnoreCase);

        if (!isAdmin && !isEmployee)
        {
            AddError(
                "User",
                "You are not authorized to access the dashboard.");

            await Send.ErrorsAsync(403, ct);
            return;
        }

        var tickets = await connection.QueryAsync<Response>(
            sql,
            new
            {
                IsAdmin = isAdmin,
                IsEmployee = isEmployee,
                UserId = _currentUser.UserId
            });

        await Send.OkAsync(
            tickets.ToList(),
            ct);
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

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}