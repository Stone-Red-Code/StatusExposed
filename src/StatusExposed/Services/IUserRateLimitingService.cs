using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IUserRateLimitingService
{
    /// <summary>
    /// Loads all rate limiting rules of all users.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task LoadAllAsync();

    /// <summary>
    /// Adds a new user to the rate limiting service.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>

    Task AddUserAsync(User user);

    /// <summary>
    ///
    /// </summary>
    /// <param name="user"></param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task UpdateUserAsync(User user);
}