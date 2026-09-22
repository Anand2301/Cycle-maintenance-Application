using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetPriorities;

public class GetPrioritiesEndpoint
    : EndpointWithoutRequest<List<GetPrioritiesEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetPrioritiesEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/priorities");
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name
            FROM Priorities
            ORDER BY Name;
            """;

        var priorities = await connection.QueryAsync<Response>(sql);

        await Send.OkAsync(
            priorities.ToList(),
            ct);
    }

    public class Response
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}