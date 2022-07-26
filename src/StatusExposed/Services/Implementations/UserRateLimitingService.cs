using AspNetCoreRateLimit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using StatusExposed.Database;
using StatusExposed.Models;

namespace StatusExposed.Services.Implementations;

public class UserRateLimitingService : IUserRateLimitingService
{
    private readonly ClientRateLimitOptions clientRateLimitOptions;
    private readonly IClientPolicyStore clientPolicyStore;
    private readonly ILogger<UserRateLimitingService> logger;
    private readonly DatabaseContext mainDatabaseContext;

    public UserRateLimitingService(IOptions<ClientRateLimitOptions> clientRateLimitOptions, IClientPolicyStore clientPolicyStore, ILogger<UserRateLimitingService> logger, DatabaseContext mainDatabaseContext)
    {
        this.clientRateLimitOptions = clientRateLimitOptions.Value;
        this.clientPolicyStore = clientPolicyStore;
        this.logger = logger;
        this.mainDatabaseContext = mainDatabaseContext;
    }

    public async Task LoadAllAsync()
    {
        IEnumerable<User> users;
        int count = 0;

        logger.LogInformation("Loading rate limit rules");

        do
        {
            users = mainDatabaseContext.Users
                .AsNoTracking()
                .Skip(count * 1000)
                .Take(1000)
                .AsEnumerable();

            foreach (User user in users)
            {
                //do stuff with your entity

                string? id = $"{clientRateLimitOptions.ClientPolicyPrefix}_{user.Id}";
                ClientRateLimitPolicy clientRateLimitPolicy = new ClientRateLimitPolicy
                {
                    ClientId = user.Id.ToString()
                };

                foreach (RateLimitRule? generalRule in clientRateLimitOptions.GeneralRules)
                {
                    RateLimitRule rateLimitRule = new RateLimitRule()
                    {
                        Endpoint = generalRule.Endpoint,
                        Limit = generalRule.Limit,
                        MonitorMode = generalRule.MonitorMode,
                        Period = generalRule.Period,
                        PeriodTimespan = generalRule.PeriodTimespan,
                    };

                    clientRateLimitPolicy.Rules.Add(rateLimitRule);
                }

                await clientPolicyStore.SetAsync(id, clientRateLimitPolicy);
            }
            count++;
        } while (users.Any());

        logger.LogInformation("Finished loading rate limit rules");
    }

    public Task AddUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}