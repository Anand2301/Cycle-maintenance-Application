namespace Cycle_App.Features.Tickets.UpdateTicket;

using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;
using global::Cycle_App.Data;


public class UpdateTicketEndpoint
    : Endpoint<UpdateTicketRequest, UpdateTicketResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ICurrentUserService _currentUser;

    public UpdateTicketEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        ICurrentUserService currentUser)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _currentUser = currentUser;
    }

    public override void Configure()
    {
        Put("/api/tickets/{TicketId}");
    }

    public override async Task HandleAsync(
        UpdateTicketRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE Tickets
            SET
                StatusId = COALESCE(@StatusId, StatusId),
                PriorityId = COALESCE(@PriorityId, PriorityId),
                AssigneeId = COALESCE(@AssigneeId, AssigneeId),
                ContactPhone = COALESCE(@ContactPhone, ContactPhone),
                ContactEmail = COALESCE(@ContactEmail, ContactEmail),
                Description = COALESCE(@Description, Description),
                UpdatedAt = NOW(),
                UDF1= COALESCE(@UDF1,UDF1), 
                UDF2= COALESCE(@UDF2,UDF2), 
                UDF3= COALESCE(@UDF3,UDF3), 
                UDF4= COALESCE(@UDF4,UDF4), 
                UDF5= COALESCE(@UDF5,UDF5), 
                UDF6= COALESCE(@UDF6,UDF6), 
                UDF7= COALESCE(@UDF7,UDF7), 
                UDF8= COALESCE(@UDF8,UDF8), 
                UDF9= COALESCE(@UDF9,UDF9), 
                UDF10= COALESCE(@UDF10,UDF10), 
                UpdatedBy = @UpdatedBy
            WHERE Id = @TicketId
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
                CreatedAt,
                UpdatedAt,
                UpdatedBy;
            """;

        var ticket = await connection.QuerySingleOrDefaultAsync<UpdateTicketResponse>(
            sql,
            new
            {
                req.TicketId,
                req.StatusId,
                req.PriorityId,
                req.AssigneeId,
                req.ContactPhone,
                req.ContactEmail,
                req.Description,
                req.UDF1,
                req.UDF2,
                req.UDF3,
                req.UDF4,
                req.UDF5,
                req.UDF6,
                req.UDF7,
                req.UDF8,
                req.UDF9,
                req.UDF10,
                UpdatedBy = _currentUser.UserId
            });

        if (ticket is null)
        {
            AddError(
                "TicketId",
                "Ticket not found.");

            await Send.ErrorsAsync(404, ct);
            return;
        }

        await Send.OkAsync(ticket, ct);
    }
}