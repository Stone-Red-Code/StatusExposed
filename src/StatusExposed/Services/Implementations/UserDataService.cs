using Microsoft.EntityFrameworkCore;

using StatusExposed.Database;
using StatusExposed.Models;

namespace StatusExposed.Services.Implementations;

public class UserDataService : IUserDataService
{
    private readonly DatabaseContext mainDatabaseContext;
    private readonly ILogger<UserDataService> logger;
    private readonly IAuthenticationService authenticationService;

    public UserDataService(DatabaseContext mainDatabaseContext, ILogger<UserDataService> logger, IAuthenticationService authenticationService)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
        this.authenticationService = authenticationService;
    }

    public async Task<bool> SubscribeToServiceAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        ServiceInformation? statusInformation = await mainDatabaseContext.Services
                                                        .Include(s => s.Subscribers)
                                                        .Where(s => s.ServicePageDomain == domain)
                                                        .FirstOrDefaultAsync();

        if (statusInformation is null)
        {
            logger.LogError("Tried to subscribe to a non existing service ({service}).", domain);
            return false;
        }

        User? user = await authenticationService.GetUserAsync();

        if (user is null)
        {
            logger.LogError("Anonymous can't subscribe to a service!");
            return false;
        }

        if (statusInformation.Subscribers.Any(s => s.Email == user.Email))
        {
            return false;
        }

        statusInformation.Subscribers.Add(new Subscriber(user.Email));

        await mainDatabaseContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnsubscribeFromServiceAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        ServiceInformation? statusInformation = await mainDatabaseContext.Services
                                                        .Include(s => s.Subscribers)
                                                        .Where(s => s.ServicePageDomain == domain)
                                                        .FirstOrDefaultAsync();

        if (statusInformation is null)
        {
            logger.LogError("Tried to unsubscribe from a non existing service ({service}).", domain);
            return false;
        }

        User? user = await authenticationService.GetUserAsync();

        if (user is null)
        {
            logger.LogError("Anonymous can't unsubscribe from a service!");
            return false;
        }

        statusInformation.Subscribers = statusInformation.Subscribers.Where(u => u.Email != user.Email).ToList();

        await mainDatabaseContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ServiceInformation>?> GetAllSubscribedServicesAsync()
    {
        User? user = await authenticationService.GetUserAsync();

        if (user is null)
        {
            logger.LogError("Anonymous can't retrieve a list of all subscribed services!");
            return null;
        }

        return mainDatabaseContext.Services.Include(s => s.Subscribers).Where(s => s.Subscribers.Any(s => s.Email == user.Email));
    }
}