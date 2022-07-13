using AspNetCoreRateLimit;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using StatusExposed.Database;
using StatusExposed.Services;
using StatusExposed.Services.Implementations;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IDatabaseConfiguration>(new DatabaseConfiguration(builder.Configuration["DatabasePath"]));
builder.Services.AddSingleton<IScheduledUpdateService, ScheduledUpdateService>();
builder.Services.AddScoped<DatabaseContext>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddBlazorise(options => { options.Immediate = true; });
builder.Services.AddBootstrapProviders();
builder.Services.AddFontAwesomeIcons();

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseIpRateLimiting();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Services.GetServices<IScheduledUpdateService>().First().Start(TimeSpan.FromMinutes(10));

using (IServiceScope scope = app.Services.CreateScope())
using (DatabaseContext? context = scope.ServiceProvider.GetService<DatabaseContext>())
{
    ILogger? logger = scope.ServiceProvider.GetService<ILogger>();
    if (context?.Database.EnsureCreated() == true)
    {
        logger?.LogInformation("Automatically created database because it didn't exist.");
    }
}

app.Run();