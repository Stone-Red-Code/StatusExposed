using StatusExposed.Database;
using StatusExposed.Models;

using System.Diagnostics;

namespace StatusExposed.Services.Implementations;

public class StatusService : IStatusService
{
    private readonly DatabaseContext mainDatabaseContext;
    public readonly ILogger<StatusService> logger;

    public StatusService(DatabaseContext mainDatabaseContext, ILogger<StatusService> logger)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
    }

    public async Task<StatusInformation?> GetStatusAsync(string domain)
    {
        domain = domain.Trim().ToLower();
        await UpdateStatus(domain);
        return await mainDatabaseContext.Services.FindAsync(domain);
    }

    public async Task AddServiceAsync(string domain, string? statusPageUrl)
    {
        domain = domain.Trim().ToLower();

        if (mainDatabaseContext.Services.Any(s => s.ServicePageDomain == domain))
        {
            return;
        }

        StatusInformation statusInformation = new StatusInformation()
        {
            ServicePageDomain = domain,
            StatusPageUrl = statusPageUrl,
            LastUpdateTime = DateTime.MinValue
        };

        _ = await mainDatabaseContext.Services.AddAsync(statusInformation);
        _ = await mainDatabaseContext.SaveChangesAsync();

        await UpdateStatus(domain);
    }

    private async Task UpdateStatus(string domain)
    {
        domain = domain.Trim().ToLower();

        StatusInformation? statusInformation = await mainDatabaseContext.Services.FindAsync(domain);

        if (statusInformation is null || (DateTime.UtcNow - statusInformation.LastUpdateTime < TimeSpan.FromMinutes(10)))
        {
            return;
        }

        HttpClient client = new HttpClient();

        await CheckUrl($"https://{domain}");

        if (statusInformation.Status != Status.Up)
        {
            await CheckUrl($"http://{domain}");
        }

        statusInformation.LastUpdateTime = DateTime.UtcNow;

        _ = await mainDatabaseContext.SaveChangesAsync();

        async Task CheckUrl(string url)
        {
            if (statusInformation is null)
            {
                return;
            }

            try
            {
                Stopwatch stopWatch = Stopwatch.StartNew();
                HttpResponseMessage? response = await client.GetAsync(url);
                stopWatch.Stop();
                statusInformation.Ping = TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds);

                if (response?.IsSuccessStatusCode == true)
                {
                    statusInformation.Status = Status.Up;
                }
                else
                {
                    statusInformation.Status = Status.Down;
                }
            }
            catch (HttpRequestException)
            {
                logger.LogDebug("HTTP request failed for {url}", url);

                statusInformation.Status = Status.Down;
                statusInformation.Ping = TimeSpan.MaxValue;
            }
        }
    }

    public IEnumerable<StatusInformation> GetStatuses(int index, int count)
    {
        return mainDatabaseContext.Services.OrderByDescending(s => s.LastUpdateTime).Skip(index).Take(count);
    }
}