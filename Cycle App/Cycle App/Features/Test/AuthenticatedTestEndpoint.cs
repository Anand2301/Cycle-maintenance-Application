using System.Security.Claims;
using FastEndpoints;

namespace Cycle_App.Features.Test.Authenticated;

public class AuthenticatedTestEndpoint
    : EndpointWithoutRequest<object>
{
    public override void Configure()
    {
        Get("/api/test/authenticated");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.Identity?.Name;

        await Send.OkAsync(
            new
            {
                Message = "JWT authentication is working.",
                UserId = userId,
                Email = email,
                Name = name
            },
            ct);
    }
}