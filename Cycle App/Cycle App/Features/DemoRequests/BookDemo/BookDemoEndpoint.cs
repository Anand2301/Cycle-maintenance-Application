using Cycle_App.Data;
using Cycle_App.Services.WhatsApp;
using Dapper;
using FastEndpoints;

namespace Cycle_App.Features.DemoRequests.BookDemo;

public class BookDemoEndpoint: Endpoint<BookDemoRequest,BookDemoResponse>
{
    private readonly IDbConnectionFactory _dbconnectionfactory;
    private readonly IWhatsAppService _whatsappservice;
    public BookDemoEndpoint(IDbConnectionFactory dbconnectionfactory,
        IWhatsAppService whatsAppService)
    {
        _dbconnectionfactory = dbconnectionfactory;
        _whatsappservice = whatsAppService;
    }
    public override void Configure()
    {
        Post("/api/demo-requests");
        AllowAnonymous();
    }
    public override async Task HandleAsync(BookDemoRequest request,CancellationToken cancellationToken)
    {
        using var connection = _dbconnectionfactory.CreateConnection();

        const string sql = """
                            INSERT INTO DemoRequests
                            (
                            Name,
                            PhoneNumber,
                            Email,
                            Address
                            )
                            VALUES
                            (
                            @Name,
                            @PhoneNumber,
                            @Email,
                            @Address
                            )
                            RETURNING Id;
                            """;
        var id = await connection.QuerySingleAsync<Guid>(sql, new
        {
            request.Name,
            request.PhoneNumber,
            request.Email,
            request.Address
        });
        var message = $"""
            New Demo Request

            Name : {request.Name}
            Phone: {request.PhoneNumber}
            Email: {request.Email}
            Address: {request.Address}

            please contact the customer for a demo
            """;
        var whatsAppUrl = _whatsappservice.GenerateChatUrl("SERVICE_CENTER_WHATSAPP_NUMBER", message);
        await Send.OkAsync(
            new BookDemoResponse
            {
                Id = id,
                Message = "Demo Request created successfully",
                WhatsAppUrl = whatsAppUrl,
            },cancellationToken);
    }
}
