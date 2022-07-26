namespace StatusExposed.Services.Implementations;

public class ScheduledUpdateService : IHostedService
{
    private readonly ILogger logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IConfiguration configuration;
    private Timer? timer;

    public ScheduledUpdateService(ILogger<ScheduledUpdateService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
        this.configuration = configuration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting scheduled update service");

        timer = new Timer((_) => UpdateServices(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping scheduled update service");

        timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private void UpdateServices()
    {
        if (!configuration.GetValue<bool>("AutomaticUpdates"))
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IStatusService statusService = scope.ServiceProvider.GetRequiredService<IStatusService>();

        IEnumerable<string> domains;
        int count = 0;

        do
        {
            // Request services in chunks
            domains = statusService
                .GetStatuses(count * 1000, 1000)
                .Select(s => s.ServicePageDomain);

            Parallel.ForEach(domains, async (domain) =>
            {
                logger.LogDebug("Updating {domain} ...", domain);
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                IStatusService statusService = scope.ServiceProvider.GetRequiredService<IStatusService>();
                await statusService.UpdateStatusAsync(domain);
                logger.LogDebug("Updated: {domain}", domain);
            });
            count++;
        }
        while (domains.Any());
    }
}