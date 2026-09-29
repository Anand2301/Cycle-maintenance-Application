namespace CycleMaintenance.Services.Authentication;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Email { get; }
    string? Name { get; }
    string? Role { get; }
}