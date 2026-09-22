using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetRoles;

public class GetRolesEndpoint
    : EndpointWithoutRequest<List<GetRolesEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetRolesEndpoint(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/roles");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name,
                Description
            FROM Roles
            WHERE IsActive = TRUE
            ORDER BY Name;
            """;

        var roles = await connection.QueryAsync<Response>(sql);

        await Send.OkAsync(roles.ToList(), ct);
    }

    public class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}