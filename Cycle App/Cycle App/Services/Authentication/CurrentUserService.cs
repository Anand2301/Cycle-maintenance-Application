using System.Security.Claims;

namespace CycleMaintenance.Services.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return Guid.TryParse(userId, out var id)
                ? id
                : Guid.Empty;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirst(ClaimTypes.Email)?
            .Value;

    public string? Name =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .Name;

    public string? Role =>
        _httpContextAccessor.HttpContext?
        .User
        .FindFirst(ClaimTypes.Role)?
        .Value;
}
