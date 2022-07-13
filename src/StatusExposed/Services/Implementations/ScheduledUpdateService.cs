using StatusExposed.Models;

namespace StatusExposed.Services.Implementations;

public class ScheduledUpdateService : IScheduledUpdateService
{
    private readonly ILogger logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IConfiguration configuration;
    private readonly System.Timers.Timer timer;

    public ScheduledUpdateService(ILogger<ScheduledUpdateService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
        this.configuration = configuration;
        timer = new System.Timers.Timer();
        timer.Elapsed += Timer_Elapsed;
    }

    public void Start(TimeSpan updateRate)
    {
        timer.Interval = updateRate.TotalMilliseconds;
        timer.Start();
        Timer_Elapsed(timer, null!);
    }

    public void Stop()
    {
        timer.Stop();
    }

    private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (!configuration.GetValue<bool>("AutomaticUpdates"))
        {
            return;
        }

        timer.Enabled = false;

        using IServiceScope? scope = serviceScopeFactory.CreateScope();
        IStatusService? statusService = scope.ServiceProvider.GetService<IStatusService>();

        if (statusService is null)
        {
            logger.LogError("Cannot find status service!");
            return;
        }

        IEnumerable<StatusInformation> statusInformations;
        int count = 0;

        do
        {
            // Request services in chunks
            statusInformations = statusService.GetStatuses(count * 100, count * 100 + 100);

            foreach (string servicePageDomain in statusInformations.Select(s => s.ServicePageDomain))
            {
                logger.LogDebug("Updating {domain} ...", servicePageDomain);
                await statusService.UpdateStatusAsync(servicePageDomain);
                logger.LogDebug("Updated: {domain}", servicePageDomain);
            }
            count++;
        }
        while (statusInformations.Any());

        timer.Enabled = true;
    }
}