using Microsoft.EntityFrameworkCore;

using StatusExposed.Database;
using StatusExposed.Models;

using System.Diagnostics;

namespace StatusExposed.Services.Implementations;

public class StatusService : IStatusService
{
    private readonly DatabaseContext mainDatabaseContext;
    public readonly ILogger<StatusService> logger;
    private readonly IConfiguration configuration;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public StatusService(DatabaseContext mainDatabaseContext, ILogger<StatusService> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
        this.configuration = configuration;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    ///<inheritdoc cref="IStatusService.GetStatusAsync(string)(string, string?)"/>
    public async Task<ServiceInformation?> GetStatusAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        // Only manually update if automatic updates are disabled
        if (!configuration.GetValue<bool>("AutomaticUpdates"))
        {
            await UpdateStatusAsync(domain);
        }

        return await mainDatabaseContext.Services
            .Include(s => s.StatusHistory)
            .FirstOrDefaultAsync(s => s.ServicePageDomain == domain);
    }

    ///<inheritdoc cref="IStatusService.AddServiceAsync(string, string?)"/>
    public async Task AddServiceAsync(string domain, string? statusPageUrl)
    {
        domain = domain.Trim().ToLower();

        ServiceInformation? statusInformation = await mainDatabaseContext.Services.FindAsync(domain);

        if (statusInformation is not null)
        {
            logger.LogWarning("Tried to add already existing service: {domain}", domain);
            statusInformation.StatusPageUrl = statusPageUrl;
            _ = await mainDatabaseContext.SaveChangesAsync();
            return;
        }

        statusInformation = new ServiceInformation()
        {
            ServicePageDomain = domain,
            StatusPageUrl = statusPageUrl,
        };

        await UpdateStatusAsync(statusInformation, true);
    }

    ///<inheritdoc cref="IStatusService.GetStatuses(int, int)"/>
    public IEnumerable<ServiceInformation> GetStatuses(int index, int count)
    {
        return mainDatabaseContext.Services.Include(s => s.StatusHistory)
            .AsSplitQuery()
            .OrderByDescending(s => s.StatusHistory
                .OrderByDescending(s => s.LastUpdateTime)
                .First().LastUpdateTime).Skip(index)
            .Take(count);
    }

    ///<inheritdoc cref="IStatusService.UpdateStatusAsync(string)"/>
    public async Task UpdateStatusAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        ServiceInformation? statusInformation = await mainDatabaseContext.Services
            .Include(s => s.StatusHistory)
            .Include(s => s.Subscribers)
            .FirstOrDefaultAsync(s => s.ServicePageDomain == domain);

        await UpdateStatusAsync(statusInformation);
    }

    private async Task UpdateStatusAsync(ServiceInformation? statusInformation, bool addNew = false)
    {
        if (statusInformation is null || (DateTime.UtcNow - statusInformation.CurrentStatus.LastUpdateTime < TimeSpan.FromMinutes(10)))
        {
            return;
        }

        Status oldStatus = statusInformation.CurrentStatus.Status;

        StatusData statusHistoryData = new StatusData()
        {
            LastUpdateTime = DateTime.MaxValue
        };

        HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        await CheckUrl($"https://{statusInformation.ServicePageDomain}");

        if (statusHistoryData.Status != Status.Up)
        {
            await CheckUrl($"http://{statusInformation.ServicePageDomain}");
        }

        statusHistoryData.LastUpdateTime = DateTime.UtcNow;

        statusInformation.StatusHistory.Add(statusHistoryData);

        // Limit history entries to 144 (10 min * 100 entries = 24h history)
        statusInformation.StatusHistory = statusInformation.StatusHistory
            .OrderByDescending(h => h.LastUpdateTime)
            .Take(144).ToList();

        if (addNew)
        {
            mainDatabaseContext.Add(statusInformation);
        }

        _ = await mainDatabaseContext.SaveChangesAsync();

        if (oldStatus != statusHistoryData.Status && !addNew)
        {
            logger.LogDebug("Status of {service} changed", statusInformation.ServicePageDomain);

            // Fire and forget (but with logs) xD
            _ = Task.Run(() =>
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                IEmailService? emailService = scope.ServiceProvider.GetService<IEmailService>();

                if (emailService is null)
                {
                    logger.LogError("{service} does not exist!", nameof(IEmailService));
                    return;
                }

                logger.LogDebug("Sending emails to {count} accounts. ({service})", statusInformation.Subscribers.Count, statusInformation.ServicePageDomain);

                if (statusInformation.Subscribers.Count > 0)
                {
                    emailService.Send(statusInformation.Subscribers.Select(s => s.Email), $"{statusInformation.ServicePageDomain} is {statusInformation.CurrentStatus.Status}", "This email has not body");
                }

                logger.LogDebug("Finished sending emails to {count} accounts. ({service})", statusInformation.Subscribers.Count, statusInformation.ServicePageDomain);
            });
        }

        // Check if URL is reachable.
        async Task CheckUrl(string url)
        {
            if (statusInformation is null)
            {
                return;
            }

            try
            {
                Stopwatch pingStopWatch = Stopwatch.StartNew();
                HttpResponseMessage? response = await client.GetAsync(url);
                pingStopWatch.Stop();
                statusHistoryData.ResponseTime = TimeSpan.FromMilliseconds(pingStopWatch.ElapsedMilliseconds);

                if (response?.IsSuccessStatusCode == true)
                {
                    statusHistoryData.Status = Status.Up;
                }
                else
                {
                    statusHistoryData.Status = Status.Down;
                }
            }
            catch (HttpRequestException)
            {
                logger.LogDebug("HTTP request failed for {url}", url);

                statusHistoryData.Status = Status.Down;
                statusHistoryData.ResponseTime = TimeSpan.MaxValue;
            }
        }
    }
}