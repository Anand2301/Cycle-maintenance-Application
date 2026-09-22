using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetCustomers;

public class GetCustomersEndpoint
    : EndpointWithoutRequest<List<GetCustomersEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCustomersEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/customers");
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name,
                PhoneNumber,
                Email,
                Address
            FROM Customers
            ORDER BY Name;
            """;

        var customers = await connection.QueryAsync<Response>(sql);

        await Send.OkAsync(
            customers.ToList(),
            ct);
    }

    public class Response
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}