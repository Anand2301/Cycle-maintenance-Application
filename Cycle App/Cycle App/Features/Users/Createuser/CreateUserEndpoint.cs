using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;

namespace CycleMaintenance.Features.Users.CreateUser;

public class CreateUserEndpoint
    : Endpoint<CreateUserRequest, CreateUserResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccountActivationService _accountActivationService;

    public CreateUserEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        IPasswordHasher passwordHasher,
        IAccountActivationService accountActivationService)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _passwordHasher = passwordHasher;
        _accountActivationService = accountActivationService;
    }

    public override void Configure()
    {
        Post("/api/users");

        // Temporary during development.
        // Later this endpoint will be restricted to Admin users.
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        CreateUserRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        // 1. Hash the user's password.
        var passwordHash = _passwordHasher.HashPassword(req.Password);

        // 2. Create the user as inactive.
        const string sql = """
            INSERT INTO Users
            (
                FirstName,
                LastName,
                Email,
                PhoneNumber,
                PasswordHash,
                IsActive
            )
            VALUES
            (
                @FirstName,
                @LastName,
                @Email,
                @PhoneNumber,
                @PasswordHash,
                FALSE
            )
            RETURNING
                Id,
                FirstName,
                LastName,
                Email,
                PhoneNumber,
                IsActive,
                CreatedAt;
            """;

        var user = await connection.QuerySingleAsync<CreateUserResponse>(
            sql,
            new
            {
                req.FirstName,
                req.LastName,
                req.Email,
                req.PhoneNumber,
                PasswordHash = passwordHash
            });

        // 3. Generate an account activation token.
        var activationToken =
            await _accountActivationService.CreateActivationTokenAsync(
                user.Id,
                ct);

        // Temporary development output.
        Console.WriteLine(
            $"Activation token for user {user.Id}: {activationToken}");

        // 4. Return the newly created user.
        await Send.OkAsync(user, ct);
    }
}