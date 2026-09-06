using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;
using System.Security.Cryptography;
using System.Text;

namespace CycleMaintenance.Features.Authentication.ActivateAccount;

public class ActivateAccountEndpoint
    : Endpoint<ActivateAccountRequest, ActivateAccountResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IPasswordHasher _passwordHasher;

    public ActivateAccountEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        IPasswordHasher passwordHasher)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _passwordHasher = passwordHasher;
    }

    public override void Configure()
    {
        Post("/api/auth/activate");
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        ActivateAccountRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        // 1. Convert the token received from the user
        //    back into bytes.
        byte[] tokenBytes;

        try
        {
            tokenBytes = Convert.FromBase64String(req.Token);
        }
        catch
        {
            AddError("Token", "Invalid activation token.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        // 2. Hash the token.
        //    We compare this hash with the one stored in UserTokens.
        var tokenHash = Convert.ToBase64String(
            SHA256.HashData(tokenBytes));

        // 3. Find the activation token.
        const string findTokenSql = """
            SELECT
                Id,
                UserId,
                ExpiresAt,
                UsedAt
            FROM UserTokens
            WHERE TokenHash = @TokenHash
              AND TokenType = 'AccountActivation';
            """;

        var tokenRecord =
            await connection.QuerySingleOrDefaultAsync<ActivationTokenRecord>(
                findTokenSql,
                new
                {
                    TokenHash = tokenHash
                });

        // 4. Make sure the token exists.
        if (tokenRecord is null)
        {
            AddError("Token", "Invalid activation token.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        // 5. Make sure the token hasn't already been used.
        if (tokenRecord.UsedAt.HasValue)
        {
            AddError("Token", "This activation token has already been used.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        // 6. Make sure the token hasn't expired.
        if (tokenRecord.ExpiresAt <= DateTime.UtcNow)
        {
            AddError("Token", "This activation token has expired.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        // 7. Hash the user's new password.
        var passwordHash =
            _passwordHasher.HashPassword(req.NewPassword);

        // 8. Update the user and mark the token as used.
        const string updateSql = """
            UPDATE Users
            SET
                PasswordHash = @PasswordHash,
                IsActive = TRUE,
                UpdatedAt = NOW()
            WHERE Id = @UserId;

            UPDATE UserTokens
            SET UsedAt = NOW()
            WHERE Id = @TokenId;
            """;

        await connection.ExecuteAsync(
            updateSql,
            new
            {
                PasswordHash = passwordHash,
                UserId = tokenRecord.UserId,
                TokenId = tokenRecord.Id
            });

        // 9. Tell the user that activation succeeded.
        await Send.OkAsync(
            new ActivateAccountResponse
            {
                Message = "Account activated successfully."
            },
            ct);
    }

    private class ActivationTokenRecord
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}