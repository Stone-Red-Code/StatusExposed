using Microsoft.EntityFrameworkCore;

using StatusExposed.Database;
using StatusExposed.Models;

namespace StatusExposed.Services.Implementations;

public class AdminDataService : IAdminDataService
{
    private readonly DatabaseContext mainDatabaseContext;
    private readonly IAuthorizationService authorizationService;

    public AdminDataService(DatabaseContext mainDatabaseContext, IAuthorizationService authorizationService)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.authorizationService = authorizationService;
    }

    public async Task<int> GetAmountOfServicesAsync()
    {
        if (!await authorizationService.IsAuthorized("role:admin"))
        {
            return -1;
        }

        return mainDatabaseContext.Services.Count();
    }

    public async Task<int> GetAmountOfUsersAsync(bool? verified = null)
    {
        if (!await authorizationService.IsAuthorized("role:admin"))
        {
            return -1;
        }

        if (verified is null)
        {
            return await mainDatabaseContext.Users.CountAsync();
        }
        else
        {
            return await mainDatabaseContext.Users.Where(u => u.IsVerified == verified).CountAsync();
        }
    }

    public async Task<User?> GetUserInfoAsync(string email)
    {
        return await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task LogOutUserAsync(string email)
    {
        throw new NotImplementedException();
    }
}