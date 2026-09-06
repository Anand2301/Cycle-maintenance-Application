using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Test;

public class DatabaseTestEndpoint : EndpointWithoutRequest<string>
{
    private readonly IDbConnectionFactory _dbconnectionfactory;

    public DatabaseTestEndpoint(IDbConnectionFactory dbconnectionfactory)
    {
        _dbconnectionfactory = dbconnectionfactory;
    }
    public override void Configure()
    {
        Get("/api/test/database");
        AllowAnonymous();
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbconnectionfactory.CreateConnection();
        var result = await connection.QuerySingleAsync<int>("Select 1");

        await Send.OkAsync($"Database Connection Successful. Result : {result}",ct);

    }
}
