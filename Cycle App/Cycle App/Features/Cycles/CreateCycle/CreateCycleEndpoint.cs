using Cycle_App.Data;
using Cycle_App.Features.Cycles.CreateCycle;
using Dapper;
using FastEndpoints;

namespace CycleMaintenance.Features.Cycles.CreateCycle;

public class CreateCycleEndpoint
    : Endpoint<CreateCycleRequest, CreateCycleResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CreateCycleEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Post("/api/cycles");
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        CreateCycleRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Cycles
            (
                CustomerId,
                Model,
                SerialNumber
            )
            VALUES
            (
                @CustomerId,
                @Model,
                @SerialNumber
            )
            RETURNING
                Id,
                CustomerId,
                Model,
                SerialNumber,
                QrToken,
                CreatedAt;
            """;

        var cycle = await connection.QuerySingleAsync<CreateCycleResponse>(
            sql,
            req);

        await Send.OkAsync(cycle, ct);
    }
}