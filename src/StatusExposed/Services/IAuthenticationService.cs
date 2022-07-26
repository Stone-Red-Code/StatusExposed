using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAuthenticationService
{
    Task<User?> GetUserAsync();

    Task RegisterUserAsync(string email);

    Task<(bool Success, string? Message)> LoginUserAsync(string email);

    Task LogoutUserAsync();

    Task<bool> VerifyUserAsync(string mailToken);

    Task DeleteRequestUserAsync();

    Task<bool> DeleteUserAsync(string deletionToken);

    Task<bool> UserExistsAsync(string email);

    Task<bool> IsAuthenticated();
}