using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAdminDataService
{
    /// <summary>
    /// Gets the user by their E-mail address.
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and the requested user or <see langword="null"/> if it can't be found!</returns>
    Task<User?> GetUserInfoAsync(string email);

    /// <summary>
    ///
    /// </summary>
    /// <param name="email"></param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task DeleteUserAsync(string email);

    /// <summary>
    /// Gets the amount of the registered users.
    /// </summary>
    /// <param name="verified">Specifies if the users should be verified or not.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and the amount of verified and/or unverified users.</returns>
    Task<int> GetAmountOfUsersAsync(bool? verified = null);

    /// <summary>
    /// Gets the current amount of registered services.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and the current amount of registered services.</returns>
    Task<int> GetAmountOfServicesAsync();

    /// <summary>
    /// Adds a permission to the specified user.
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <param name="permission">The new permission.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task AddPermissionToUserAsync(string email, Permission permission);

    /// <summary>
    ///
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <param name="permission">The permission to be removed.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task RemovePermissionFromUserAsync(string email, Permission permission);

    /// <summary>
    /// Sets the ban property of the specified user.
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <param name="isBanned">The ban property.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task SetUserBan(string email, bool isBanned);

    /// <summary>
    /// Purges all unverified users.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task PurgeUnverifiedUsersAsync();
}