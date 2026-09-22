namespace CycleMaintenance.Features.Authentication.Login;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public List<RoleResponse> Roles { get; set; } = [];
}

public class RoleResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}