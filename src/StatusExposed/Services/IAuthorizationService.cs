using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAuthorizationService
{
    /// <summary>
    /// Check if the current user is authorized.
    /// </summary>
    /// <param name="requiredPermissions">The required permissions in form of a <see cref="string"/>. Permissions are separated by spaces.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user is authenticated.</returns>
    Task<bool> IsAuthorized(string? requiredPermissions);

    /// <summary>
    /// Check if the current user is authorized.
    /// </summary>
    /// <param name="requiredPermissions">The required permissions.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user is authenticated.</returns>
    Task<bool> IsAuthorized(IEnumerable<Permission>? requiredPermissions);
}