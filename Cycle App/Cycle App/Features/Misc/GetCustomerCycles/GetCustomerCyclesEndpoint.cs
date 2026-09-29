using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetCustomerCycles;

public class GetCustomerCyclesEndpoint
    : Endpoint<GetCustomerCyclesEndpoint.Request,
        List<GetCustomerCyclesEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCustomerCyclesEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/customers/{CustomerId}/cycles");
    }

    public override async Task HandleAsync(
        Request req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Model,
                SerialNumber
            FROM Cycles
            WHERE CustomerId = @CustomerId
              AND IsActive = TRUE
            ORDER BY SerialNumber;
            """;

        var cycles = await connection.QueryAsync<Response>(
            sql,
            new
            {
                req.CustomerId
            });

        await Send.OkAsync(cycles.ToList(), ct);
    }

    public class Request
    {
        public Guid CustomerId { get; set; }
    }

    public class Response
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
    }
}