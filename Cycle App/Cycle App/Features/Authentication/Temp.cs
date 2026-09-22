using CycleMaintenance.Services.Authentication;
using FastEndpoints;

namespace Cycle_App.Features.Test.PasswordHash;

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
        Get("/api/test/password-hash-temp");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var password = "Test@12345";

        var hash = _passwordHasher.HashPassword(password);

        await Send.OkAsync(
            new
            {
                Password = password,
                Hash = hash
            },
            ct);
    }
}