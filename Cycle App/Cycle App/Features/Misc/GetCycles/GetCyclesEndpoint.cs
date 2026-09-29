using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetCycles;

public class GetCyclesEndpoint
    : EndpointWithoutRequest<List<GetCyclesEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCyclesEndpoint(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/cycles");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                cy.Id,
                cy.Model,
                cy.SerialNumber,
                c.Name AS CustomerName
            FROM Cycles cy
            INNER JOIN Customers c
                ON cy.CustomerId = c.Id
            WHERE cy.IsActive = TRUE
            ORDER BY cy.SerialNumber;
            """;

        var cycles = await connection.QueryAsync<Response>(sql);

        await Send.OkAsync(cycles.ToList(), ct);
    }

    public class Response
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? CustomerName { get; set; }
    }
}