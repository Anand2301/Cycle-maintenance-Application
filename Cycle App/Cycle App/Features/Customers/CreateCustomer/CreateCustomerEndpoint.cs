using Cycle_App.Data;
using Cycle_App.Features.Customers.CreateCustomer;
using Dapper;
using FastEndpoints;

namespace CycleMaintenance.Features.Customers.CreateCustomer;

public class CreateCustomerEndpoint
    : Endpoint<CreateCustomerRequest, CreateCustomerResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CreateCustomerEndpoint(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Post("/api/customers");
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        CreateCustomerRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Customers
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
            RETURNING
                Id,
                Name,
                PhoneNumber,
                Email,
                Address,
                CreatedAt;
            """;

        var customer = await connection.QuerySingleAsync<CreateCustomerResponse>(
            sql,
            req);

        await Send.OkAsync(customer, ct);
    }
}