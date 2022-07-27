using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using StatusExposed.Database;
using StatusExposed.Models;
using StatusExposed.Models.Options;

using System.Diagnostics;

namespace StatusExposed.Services.Implementations;

public class StatusService : IStatusService
{
    private readonly DatabaseContext mainDatabaseContext;
    public readonly ILogger<StatusService> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly GeneralSettings generalSettings;
    private readonly EmailSettings mailSettings;

    public StatusService(DatabaseContext mainDatabaseContext, ILogger<StatusService> logger, IServiceScopeFactory serviceScopeFactory, IOptions<GeneralSettings> generalSettings, IOptions<EmailSettings> mailSettings)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
        this.mailSettings = mailSettings.Value;
        this.generalSettings = generalSettings.Value;
    }

    ///<inheritdoc cref="IStatusService.GetStatusAsync(string)(string, string?)"/>
    public async Task<ServiceInformation?> GetStatusAsync(string domain)
    {
        domain = domain.Trim().ToLower();

        // Only manually update if automatic updates are disabled
        if (!generalSettings.AutomaticUpdates)
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
            .AsSingleQuery()
            .FirstOrDefaultAsync(s => s.ServicePageDomain == domain);

        await UpdateStatusAsync(statusInformation);
    }

    private async Task UpdateStatusAsync(ServiceInformation? serviceInformation, bool addNew = false)
    {
        if (serviceInformation is null || (DateTime.UtcNow - serviceInformation.CurrentStatus.LastUpdateTime < generalSettings.UpdatePeriodTimeSpan))
        {
            return;
        }

        Status oldStatus = serviceInformation.CurrentStatus.Status;

        StatusData statusHistoryData = new StatusData()
        {
            LastUpdateTime = DateTime.MaxValue
        };

        HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        await CheckUrl($"https://{serviceInformation.ServicePageDomain}");

        if (statusHistoryData.Status != Status.Up)
        {
            await CheckUrl($"http://{serviceInformation.ServicePageDomain}");
        }

        statusHistoryData.LastUpdateTime = DateTime.UtcNow;

        serviceInformation.StatusHistory.Add(statusHistoryData);

        // Delete entries older than a year
        serviceInformation.StatusHistory = serviceInformation.StatusHistory
            .Where(h => DateTime.UtcNow - h.LastUpdateTime < TimeSpan.FromDays(365))
            .ToList();

        if (addNew)
        {
            _ = mainDatabaseContext.Add(serviceInformation);
        }

        _ = await mainDatabaseContext.SaveChangesAsync();

        if (oldStatus != statusHistoryData.Status && !addNew)
        {
            logger.LogDebug("Status of {service} changed", serviceInformation.ServicePageDomain);

            SendEmails(serviceInformation, oldStatus);
        }

        // Check if URL is reachable.
        async Task CheckUrl(string url)
        {
            if (serviceInformation is null)
            {
                return;
            }

            try
            {
                Stopwatch pingStopWatch = Stopwatch.StartNew();
                HttpResponseMessage? response = await client.GetAsync(url);
                pingStopWatch.Stop();
                statusHistoryData.ResponseTime = TimeSpan.FromMilliseconds(pingStopWatch.ElapsedMilliseconds);

                statusHistoryData.Status = response?.IsSuccessStatusCode == true ? Status.Up : Status.Down;
            }
            catch (HttpRequestException)
            {
                logger.LogDebug("HTTP request failed for {url}", url);

                statusHistoryData.Status = Status.Down;
                statusHistoryData.ResponseTime = TimeSpan.MaxValue;
            }
        }
    }

    private void SendEmails(ServiceInformation serviceInformation, Status oldStatus)
    {
        // Fire and forget (but with logs) xD
        _ = Task.Run(async () =>
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            IEmailService? emailService = scope.ServiceProvider.GetService<IEmailService>();

            if (emailService is null)
            {
                logger.LogError("{service} does not exist!", nameof(IEmailService));
                return;
            }

            logger.LogDebug("Sending emails to {count} accounts. ({service})", serviceInformation.Subscribers.Count, serviceInformation.ServicePageDomain);

            if (serviceInformation.Subscribers.Count > 0)
            {
                IEnumerable<string> subscriberAddresses = serviceInformation.Subscribers.Select(s => s.Email);

                if (File.Exists(mailSettings?.TemplatePaths?.StatusChanged))
                {
                    await emailService.SendWithTemeplateAsync(
                        subscriberAddresses,
                        $"{serviceInformation.ServicePageDomain} is {serviceInformation.CurrentStatus.Status}",
                        mailSettings.TemplatePaths.StatusChanged,
                        null,
                        new TemplateParameter("current-status", serviceInformation.CurrentStatus.Status.ToString()),
                        new TemplateParameter("old-status", oldStatus.ToString()));
                }
                else
                {
                    logger.LogWarning("Status changed E-mail template not found, using fall back template.");
                    await emailService.SendAsync(
                        subscriberAddresses,
                        $"{serviceInformation.ServicePageDomain} is {serviceInformation.CurrentStatus.Status}",
                        $"The status of {serviceInformation.ServicePageDomain} changed from {oldStatus} to {serviceInformation.CurrentStatus.Status}");
                }
            }

            logger.LogDebug("Finished sending emails to {count} accounts. ({service})", serviceInformation.Subscribers.Count, serviceInformation.ServicePageDomain);
        });
    }
}