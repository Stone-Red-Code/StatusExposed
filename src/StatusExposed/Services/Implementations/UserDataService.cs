using Microsoft.EntityFrameworkCore;

using StatusExposed.Database;
using StatusExposed.Models;
using StatusExposed.Utilities;

namespace StatusExposed.Services.Implementations;

public class UserDataService : IUserDataService
{
    private readonly DatabaseContext mainDatabaseContext;
    private readonly IAuthenticationService authenticationService;

    public UserDataService(DatabaseContext mainDatabaseContext, IAuthenticationService authenticationService)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.authenticationService = authenticationService;
    }

    public async Task<(bool Success, string? ErrorMessage)> SubscribeToServiceAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        ServiceInformation? statusInformation = await mainDatabaseContext.Services
                                                        .Include(s => s.Subscribers)
                                                        .Where(s => s.ServicePageDomain == domain)
                                                        .FirstOrDefaultAsync();

        if (statusInformation is null)
        {
            return (false, $"Site is not tracked!");
        }

        User? user = await authenticationService.GetUserAsync();

        if (user is null)
        {
            return (false, "User is null, try to login!");
        }

        if (statusInformation.Subscribers.Any(s => s.Email == user.Email))
        {
            return (false, "Already subscribed to site.");
        }

        if ((await GetAllSubscribedServicesAsync())!.Count() >= await GetSiteSubscribtionsLimitAsync())
        {
            return (false, "Max amount of subscriptions reached.");
        }

        statusInformation.Subscribers.Add(new Subscriber(user.Email));

        await mainDatabaseContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> UnsubscribeFromServiceAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        ServiceInformation? statusInformation = await mainDatabaseContext.Services
                                                        .Include(s => s.Subscribers)
                                                        .Where(s => s.ServicePageDomain == domain)
                                                        .FirstOrDefaultAsync();

        if (statusInformation is null)
        {
            return (false, $"Site is not tracked!");
        }

        User? user = await authenticationService.GetUserAsync();

        if (user is null)
        {
            return (false, "User is null, try to login!");
        }

        List<Subscriber>? entriesToDelete = statusInformation.Subscribers.Where(u => u.Email == user.Email).ToList();

        mainDatabaseContext.Subscriber.RemoveRange(entriesToDelete);

        await mainDatabaseContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task<IEnumerable<ServiceInformation>?> GetAllSubscribedServicesAsync()
    {
        User? user = await authenticationService.GetUserAsync();

        if (user is null)
        {
            return null;
        }

        return mainDatabaseContext.Services
            .Include(s => s.Subscribers)
            .Include(s => s.StatusHistory)
            .AsSplitQuery().Where(s => s.Subscribers.Any(s => s.Email == user.Email));
    }

    public async Task<int> GetSiteSubscribtionsLimitAsync()
    {
        return await Task.Run(() => 10);
    }

    public async Task GenerateNewApiKey()
    {
        User? user = await authenticationService.GetUserAsync();

        if (user is null)
        {
            return;
        }

        ApiKey apiKey = new ApiKey(TokenGenerator.GenerateToken("api", user.Id, 64));

        user.ApiKeys.Add(apiKey);

        await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task<List<ApiKey>?> GetApiKeys()
    {
        User? user = await authenticationService.GetUserAsync();

        return user?.ApiKeys;
    }
}