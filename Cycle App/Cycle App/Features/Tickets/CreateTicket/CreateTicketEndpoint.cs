using Cycle_App.Data;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.Tickets.CreateTicket;

public class CreateTicketEndpoint
    : Endpoint<CreateTicketRequest, CreateTicketResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CreateTicketEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Post("/api/tickets");
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        CreateTicketRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string validationSql = """
            SELECT EXISTS
            (
                SELECT 1
                FROM Cycles
                WHERE Id = @CycleId
                  AND CustomerId = @CustomerId
                  AND IsActive = TRUE
            );
            """;

        var cycleBelongsToCustomer =
            await connection.QuerySingleAsync<bool>(
                validationSql,
                new
                {
                    req.CycleId,
                    req.CustomerId
                });

        if (!cycleBelongsToCustomer)
        {
            AddError(
                "CycleId",
                "The selected cycle does not belong to the selected customer.");

            await Send.ErrorsAsync(400, ct);
            return;
        }

        const string sql = """
            INSERT INTO Tickets
            (
                CustomerId,
                CycleId,
                CompanyName,
                StatusId,
                PriorityId,
                AssigneeId,
                ContactPhone,
                ContactEmail,
                Description
            )
            VALUES
            (
                @CustomerId,
                @CycleId,
                @CompanyName,
                @StatusId,
                @PriorityId,
                @AssigneeId,
                @ContactPhone,
                @ContactEmail,
                @Description
            )
            RETURNING
                Id,
                CustomerId,
                CycleId,
                CompanyName,
                StatusId,
                PriorityId,
                AssigneeId,
                ContactPhone,
                ContactEmail,
                Description,
                CreatedAt;
            """;

        var ticket = await connection.QuerySingleAsync<CreateTicketResponse>(
            sql,
            req);

        await Send.OkAsync(ticket, ct);
    }
}