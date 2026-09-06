using Cycle_App.Data;
using Dapper;
using System.Security.Cryptography;
using System.Text;

namespace CycleMaintenance.Services.Authentication;

public class AccountActivationService : IAccountActivationService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public AccountActivationService(
        IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<string> CreateActivationTokenAsync(
        Guid userId,
        CancellationToken ct)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);

        var token = Convert.ToBase64String(tokenBytes);

        var tokenHash = Convert.ToBase64String(
            SHA256.HashData(tokenBytes));

        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO UserTokens
            (
                UserId,
                TokenHash,
                TokenType,
                ExpiresAt
            )
            VALUES
            (
                @UserId,
                @TokenHash,
                @TokenType,
                @ExpiresAt
            );
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                UserId = userId,
                TokenHash = tokenHash,
                TokenType = "AccountActivation",
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });

        return token;
    }
}