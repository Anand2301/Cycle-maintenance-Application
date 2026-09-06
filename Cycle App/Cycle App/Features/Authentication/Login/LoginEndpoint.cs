using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;

namespace CycleMaintenance.Features.Authentication.Login;

public class LoginEndpoint
    : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        LoginRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        // 1. Find the user by email.
        const string sql = """
            SELECT
                Id,
                FirstName,
                LastName,
                Email,
                PhoneNumber,
                PasswordHash,
                IsActive
            FROM Users
            WHERE Email = @Login;
            """;

        var user = await connection.QuerySingleOrDefaultAsync<UserLoginRecord>(
            sql,
            new
            {
                req.Login
            });

        // 2. Don't reveal whether the email exists.
        if (user is null)
        {
            AddError("Login", "Invalid login credentials.");
            await Send.ErrorsAsync(401, ct);
            return;
        }

        // 3. Check whether the account has been activated.
        if (!user.IsActive)
        {
            AddError("Login", "Invalid login credentials.");
            await Send.ErrorsAsync(401, ct);
            return;
        }

        // 4. Verify the password using Argon2id.
        if (string.IsNullOrWhiteSpace(user.PasswordHash) ||
            !_passwordHasher.VerifyPassword(
                req.Password,
                user.PasswordHash))
        {
            AddError("Login", "Invalid login credentials.");
            await Send.ErrorsAsync(401, ct);
            return;
        }

        // 5. Calculate the token expiration time.
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        // 6. Generate the JWT.
        var accessToken = _jwtService.GenerateToken(
            user.Id,
            user.Email!,
            user.FirstName);

        // 7. Return the access token.
        await Send.OkAsync(
            new LoginResponse
            {
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                FirstName = user.FirstName,
                Email = user.Email
            },
            ct);
    }

    private class UserLoginRecord
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }
    }
}