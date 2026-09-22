using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Misc.GetUsers;

public class GetUsersEndpoint
    : EndpointWithoutRequest<List<GetUsersEndpoint.Response>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetUsersEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Get("/api/misc/users");
    }

    public override async Task HandleAsync(
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                FirstName,
                LastName,
                Email
            FROM Users
            WHERE IsActive = TRUE
            ORDER BY FirstName, LastName;
            """;

        var users = await connection.QueryAsync<Response>(sql);

        await Send.OkAsync(
            users.ToList(),
            ct);
    }

    public class Response
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public string? Email { get; set; }
    }
}