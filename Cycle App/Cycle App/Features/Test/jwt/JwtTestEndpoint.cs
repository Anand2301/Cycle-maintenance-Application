using CycleMaintenance.Services.Authentication;
using FastEndpoints;

namespace CycleMaintenance.Features.Test.Jwt;

public class JwtTestEndpoint : EndpointWithoutRequest<object>
{
    private readonly IJwtService _jwtService;

    public JwtTestEndpoint(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public override void Configure()
    {
        Get("/api/test/jwt");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var token = _jwtService.GenerateToken(
            Guid.Parse("5fa058cc-314c-4795-816e-d86cef0018ba"),
            "anandkarthiswar@gmail.com",
            "Anand");

        await Send.OkAsync(
            new
            {
                Token = token
            },
            ct);
    }
}