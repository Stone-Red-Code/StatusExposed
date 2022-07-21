using StatusExposed.Models;

namespace StatusExposed.Services.Implementations;

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthenticationService authenticationService;

    public AuthorizationService(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    public async Task<bool> IsAuthorized(string? requiredPermissions)
    {
        return await IsAuthorized(
            requiredPermissions?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(p => (Permission)p));
    }

    public async Task<bool> IsAuthorized(IEnumerable<Permission>? requiredPermissions)
    {
        User? user = await authenticationService.GetUserAsync();

        if (user is null || !user.IsVerified)
        {
            return false;
        }

        if (requiredPermissions is null)
        {
            return true;
        }

        foreach (Permission permission in requiredPermissions)
        {
            if (!user.Permissions.Contains(permission))
            {
                return false;
            }
        }

        return true;
    }
}