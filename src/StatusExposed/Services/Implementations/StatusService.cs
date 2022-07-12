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

    public StatusService(DatabaseContext mainDatabaseContext, ILogger<StatusService> logger, IConfiguration configuration)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
        this.configuration = configuration;
    }

    ///<inheritdoc cref="IStatusService.GetStatusAsync(string)(string, string?)"/>
    public async Task<StatusInformation?> GetStatusAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        // Only manually update if automatic updates are disabled
        if (!configuration.GetValue<bool>("AutomaticUpdates"))
        {
            await UpdateStatusAsync(domain);
        }

        return await mainDatabaseContext.Services.FindAsync(domain);
    }

    ///<inheritdoc cref="IStatusService.AddServiceAsync(string, string?)"/>
    public async Task AddServiceAsync(string domain, string? statusPageUrl)
    {
        domain = domain.Trim().ToLower();

        StatusInformation? statusInformation = await mainDatabaseContext.Services.FindAsync(domain);

        if (statusInformation is not null)
        {
            logger.LogWarning("Tried to add already existing service: {domain}", domain);
            statusInformation.StatusPageUrl = statusPageUrl;
            _ = await mainDatabaseContext.SaveChangesAsync();
            return;
        }

        statusInformation = new StatusInformation()
        {
            ServicePageDomain = domain,
            StatusPageUrl = statusPageUrl,
        };

        _ = await mainDatabaseContext.Services.AddAsync(statusInformation);
        _ = await mainDatabaseContext.SaveChangesAsync();

        await UpdateStatusAsync(domain);
    }

    ///<inheritdoc cref="IStatusService.GetStatuses(int, int)/>
    public IEnumerable<StatusInformation> GetStatuses(int index, int count)
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

        StatusInformation? statusInformation = await mainDatabaseContext.Services
            .Include(s => s.StatusHistory)
            .FirstAsync(s => s.ServicePageDomain == domain);

        if (statusInformation is null || (DateTime.UtcNow - statusInformation.CurrentStatusHistoryData.LastUpdateTime < TimeSpan.FromMinutes(10)))
        {
            return;
        }

        statusInformation.StatusHistory.Add(new StatusHistoryData() { LastUpdateTime = DateTime.MaxValue });

        HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        await CheckUrl($"https://{domain}");

        if (statusInformation.CurrentStatusHistoryData.Status != Status.Up)
        {
            await CheckUrl($"http://{domain}");
        }

        statusInformation.CurrentStatusHistoryData.LastUpdateTime = DateTime.UtcNow;

        _ = await mainDatabaseContext.SaveChangesAsync();

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
                statusInformation.CurrentStatusHistoryData.Ping = TimeSpan.FromMilliseconds(pingStopWatch.ElapsedMilliseconds);

                if (response?.IsSuccessStatusCode == true)
                {
                    statusInformation.CurrentStatusHistoryData.Status = Status.Up;
                }
                else
                {
                    statusInformation.CurrentStatusHistoryData.Status = Status.Down;
                }
            }
            catch (HttpRequestException)
            {
                logger.LogDebug("HTTP request failed for {url}", url);

                statusInformation.CurrentStatusHistoryData.Status = Status.Down;
                statusInformation.CurrentStatusHistoryData.Ping = TimeSpan.MaxValue;
            }
        }
    }
}