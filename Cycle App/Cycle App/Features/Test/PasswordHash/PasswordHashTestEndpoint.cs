using CycleMaintenance.Services.Authentication;
using FastEndpoints;

namespace CycleMaintenance.Features.Test.PasswordHash;

public class PasswordHashTestEndpoint
    : EndpointWithoutRequest<object>
{
    private readonly IPasswordHasher _passwordHasher;

    public PasswordHashTestEndpoint(
        IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public override void Configure()
    {
        Get("/api/test/password-hash");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        const string password = "MyPassword@123";

        var hash = _passwordHasher.HashPassword(password);

        var correctPassword =
            _passwordHasher.VerifyPassword(
                "MyPassword@123",
                hash);

        var wrongPassword =
            _passwordHasher.VerifyPassword(
                "WrongPassword",
                hash);

        await Send.OkAsync(
            new
            {
                PasswordHash = hash,
                CorrectPassword = correctPassword,
                WrongPassword = wrongPassword
            },
            ct);
    }
}