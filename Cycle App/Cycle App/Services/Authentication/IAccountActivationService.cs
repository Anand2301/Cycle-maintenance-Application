namespace CycleMaintenance.Services.Authentication;

public interface IAccountActivationService
{
    Task<string> CreateActivationTokenAsync(
        Guid userId,
        CancellationToken ct);
}