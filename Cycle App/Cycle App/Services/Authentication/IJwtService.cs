namespace CycleMaintenance.Services.Authentication;

public interface IJwtService
{
    string GenerateToken(
        Guid userId,
        string email,
        string firstName);
}