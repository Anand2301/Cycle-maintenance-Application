using FastEndpoints;
namespace Cycle_App.Features.Test;

public class TestEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/api/test");
        AllowAnonymous();
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync("cycle application is working", ct);
    }
}
