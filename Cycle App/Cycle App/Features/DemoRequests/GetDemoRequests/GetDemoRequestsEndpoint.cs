using Cycle_App.Data;
using Cycle_App.Features.DemoRequests.GetDemoRequests;
using Dapper;
using FastEndpoints;

namespace CycleMaintenance.Features.DemoRequests.GetDemoRequests;

public class GetDemoRequestsEndpoint
    : EndpointWithoutRequest<List<GetDemoRequestsResponse>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetDemoRequestsEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/demo-requests");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name,
                PhoneNumber,
                Email,
                Address,
                ServiceCenterId,
                StatusId,
                CreatedAt
            FROM DemoRequests
            ORDER BY CreatedAt DESC;
            """;

        var demoRequests = await connection.QueryAsync<GetDemoRequestsResponse>(
            sql);

        await Send.OkAsync(
            demoRequests.ToList(),
            ct);
    }
}