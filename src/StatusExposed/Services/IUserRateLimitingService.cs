using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IUserRateLimitingService
{
    Task LoadAllAsync();

    Task AddUserAsync(User user);

    Task UpdateUserAsync(User user);
}