using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAuthenticationService
{
    /// <summary>
    /// Gets the current user.
    /// </summary>
    /// <returns> <see cref="Task"/> to <see langword="await"/> and the currently authenticated user.</returns>
    Task<User?> GetUserAsync();

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>

    Task RegisterUserAsync(string email);

    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task<(bool Success, string? Message)> LoginUserAsync(string email);

    /// <summary>
    /// Logs the user out.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task LogoutUserAsync();

    /// <summary>
    /// Verifies a user.
    /// </summary>
    /// <param name="mailToken"></param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the operation was successful.</returns>
    Task<bool> VerifyUserAsync(string mailToken);

    /// <summary>
    /// Sends a delete E-mail to the current user.
    /// </summary>
    /// <returns>A <see cref="Task"/> to await.</returns>
    Task DeleteRequestUserAsync();

    /// <summary>
    /// Deletes the current user.
    /// </summary>
    /// <param name="deletionToken">The deletion token.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the operation was successful.</returns>
    Task<bool> DeleteUserAsync(string deletionToken);

    /// <summary>
    /// Checks if a user exists.
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user exists.</returns>
    Task<bool> UserExistsAsync(string email);

    /// <summary>
    /// Check if the current user is authenticated.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user is authenticated.</returns>
    Task<bool> IsAuthenticated();
}