using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Dashboard.GetSummary;

public class GetDashboardSummaryEndpoint
    : EndpointWithoutRequest<GetDashboardSummaryEndpoint.Response>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardSummaryEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        ICurrentUserService currentUser)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _currentUser = currentUser;
    }

    public override void Configure()
    {
        Get("/api/dashboard/summary");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        // Admin dashboard
        if (string.Equals(
                _currentUser.Role,
                "Admin",
                StringComparison.OrdinalIgnoreCase))
        {
            const string sql = """
                SELECT
                    COUNT(*) AS TotalTickets,

                    COUNT(*) FILTER (
                        WHERE s.Name = 'New'
                    ) AS NewTickets,

                    COUNT(*) FILTER (
                        WHERE s.Name = 'Assigned'
                    ) AS AssignedTickets,

                    COUNT(*) FILTER (
                        WHERE s.Name = 'In Progress'
                    ) AS InProgressTickets,

                    COUNT(*) FILTER (
                        WHERE s.Name = 'Completed'
                    ) AS CompletedTickets,

                    COUNT(*) FILTER (
                        WHERE s.Name = 'Cancelled'
                    ) AS CancelledTickets

                FROM Tickets t
                INNER JOIN Statuses s
                    ON t.StatusId = s.Id;
                """;

            var ticketSummary =
                await connection.QuerySingleAsync<AdminTicketSummary>(sql);

            const string customerSql = """
                SELECT COUNT(*)
                FROM Customers;
                """;

            const string cycleSql = """
                SELECT COUNT(*)
                FROM Cycles
                WHERE IsActive = TRUE;
                """;

            var totalCustomers =
                await connection.ExecuteScalarAsync<int>(customerSql);

            var totalCycles =
                await connection.ExecuteScalarAsync<int>(cycleSql);

            await Send.OkAsync(
                new Response
                {
                    Role = "Admin",

                    TotalTickets = ticketSummary.TotalTickets,
                    NewTickets = ticketSummary.NewTickets,
                    AssignedTickets = ticketSummary.AssignedTickets,
                    InProgressTickets = ticketSummary.InProgressTickets,
                    CompletedTickets = ticketSummary.CompletedTickets,
                    CancelledTickets = ticketSummary.CancelledTickets,

                    TotalCustomers = totalCustomers,
                    TotalCycles = totalCycles
                },
                ct);

            return;
        }

        // Employee dashboard
        if (string.Equals(
                _currentUser.Role,
                "Employee",
                StringComparison.OrdinalIgnoreCase))
        {
            const string sql = """
                SELECT
                    COUNT(*) AS AssignedTickets,

                    COUNT(*) FILTER (
                        WHERE s.Name IN
                        (
                            'New',
                            'Assigned',
                            'In Progress'
                        )
                    ) AS PendingTickets,

                    COUNT(*) FILTER (
                        WHERE s.Name = 'Completed'
                    ) AS CompletedTickets

                FROM Tickets t
                INNER JOIN Statuses s
                    ON t.StatusId = s.Id
                WHERE t.AssigneeId = @UserId;
                """;

            var summary =
                await connection.QuerySingleAsync<EmployeeTicketSummary>(
                    sql,
                    new
                    {
                        UserId = _currentUser.UserId
                    });

            await Send.OkAsync(
                new Response
                {
                    Role = "Employee",

                    AssignedTickets = summary.AssignedTickets,
                    PendingTickets = summary.PendingTickets,
                    CompletedTickets = summary.CompletedTickets
                },
                ct);

            return;
        }

        // Other roles are not allowed to access the dashboard.
        AddError(
            "User",
            "You are not authorized to access the dashboard.");

        await Send.ErrorsAsync(403, ct);
    }

    private class AdminTicketSummary
    {
        public int TotalTickets { get; set; }

        public int NewTickets { get; set; }

        public int AssignedTickets { get; set; }

        public int InProgressTickets { get; set; }

        public int CompletedTickets { get; set; }

        public int CancelledTickets { get; set; }
    }

    private class EmployeeTicketSummary
    {
        public int AssignedTickets { get; set; }

        public int PendingTickets { get; set; }

        public int CompletedTickets { get; set; }
    }

    public class Response
    {
        public string Role { get; set; } = string.Empty;

        // Admin
        public int TotalTickets { get; set; }

        public int NewTickets { get; set; }

        public int AssignedTickets { get; set; }

        public int InProgressTickets { get; set; }

        public int CompletedTickets { get; set; }

        public int CancelledTickets { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalCycles { get; set; }

        // Employee
        public int PendingTickets { get; set; }
    }
}