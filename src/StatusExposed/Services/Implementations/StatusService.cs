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
    private readonly IConfiguration configuration;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly EmailOptions mailOptions;

    public StatusService(DatabaseContext mainDatabaseContext, ILogger<StatusService> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, IOptions<EmailOptions> mailOptions)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
        this.configuration = configuration;
        this.serviceScopeFactory = serviceScopeFactory;
        this.mailOptions = mailOptions.Value;
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

    private async Task UpdateStatusAsync(ServiceInformation? serviceInformation, bool addNew = false)
    {
        if (serviceInformation is null || (DateTime.UtcNow - serviceInformation.CurrentStatus.LastUpdateTime < TimeSpan.FromMinutes(10)))
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

        // Limit history entries to 144 (10 min * 100 entries = 24h history)
        serviceInformation.StatusHistory = serviceInformation.StatusHistory
            .OrderByDescending(h => h.LastUpdateTime)
            .Take(144).ToList();

        if (addNew)
        {
            mainDatabaseContext.Add(serviceInformation);
        }

        _ = await mainDatabaseContext.SaveChangesAsync();

        if (oldStatus != statusHistoryData.Status && !addNew)
        {
            logger.LogDebug("Status of {service} changed", serviceInformation.ServicePageDomain);

            // Fire and forget (but with logs) xD
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

    private void SendEmails(ServiceInformation serviceInformation, Status oldStatus)
    {
        _ = Task.Run(() =>
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

                if (File.Exists(mailOptions?.TemplatePaths?.StatusChanged))
                {
                    emailService.SendWithTemeplate(
                        subscriberAddresses,
                        $"{serviceInformation.ServicePageDomain} is {serviceInformation.CurrentStatus.Status}",
                        mailOptions.TemplatePaths.StatusChanged,
                        null,
                        new TemplateParameter("current-status", serviceInformation.CurrentStatus.Status.ToString()),
                        new TemplateParameter("old-status", oldStatus.ToString()));
                }
                else
                {
                    logger.LogWarning("Status changed E-Mail template not found, using fall back template.");
                    emailService.Send(
                        subscriberAddresses,
                        $"{serviceInformation.ServicePageDomain} is {serviceInformation.CurrentStatus.Status}",
                        $"The status of {serviceInformation.ServicePageDomain} changed from {oldStatus} to {serviceInformation.CurrentStatus.Status}");
                }
            }

            logger.LogDebug("Finished sending emails to {count} accounts. ({service})", serviceInformation.Subscribers.Count, serviceInformation.ServicePageDomain);
        });
    }
}