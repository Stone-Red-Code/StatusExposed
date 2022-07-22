using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAdminDataService
{
    Task<User?> GetUserInfoAsync(string email);

    Task LogOutUserAsync(string email);

    Task<int> GetAmountOfUsersAsync(bool? verified = null);

    Task<int> GetAmountOfServicesAsync();

    Task AddPermissionToUserAsync(string email, Permission permission);

    Task RemovePermissionFromUserAsync(string email, Permission permission);

    Task SetUserBan(string email, bool isBanned);
}