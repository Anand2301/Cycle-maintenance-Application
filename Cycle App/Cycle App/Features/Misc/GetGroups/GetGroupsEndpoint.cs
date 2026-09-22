using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetGroups;

public class GetGroupsEndpoint
    : EndpointWithoutRequest<List<GetGroupsEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetGroupsEndpoint(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/groups");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name,
                Description
            FROM Groups
            WHERE IsActive = TRUE
            ORDER BY Name;
            """;

        var groups = await connection.QueryAsync<Response>(sql);

        await Send.OkAsync(groups.ToList(), ct);
    }

    public class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}