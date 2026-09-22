using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetStatuses;

public class GetStatusesEndpoint
    : EndpointWithoutRequest<List<GetStatusesEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetStatusesEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/statuses");
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name
            FROM Statuses
            ORDER BY Name;
            """;

        var statuses = await connection.QueryAsync<Response>(sql);

        await Send.OkAsync(
            statuses.ToList(),
            ct);
    }

    public class Response
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}