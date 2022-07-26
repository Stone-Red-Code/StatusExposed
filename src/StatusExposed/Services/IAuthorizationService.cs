using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAuthorizationService
{
    Task<bool> IsAuthorized(string? requiredPermissions);

    Task<bool> IsAuthorized(IEnumerable<Permission>? requiredPermissions);
}