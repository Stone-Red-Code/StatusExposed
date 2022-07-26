using AspNetCoreRateLimit;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using StatusExposed.Database;
using StatusExposed.Middleware;
using StatusExposed.Models.Options;
using StatusExposed.Services;
using StatusExposed.Services.Implementations;
using StatusExposed.Utilities;

using System.Text.Json.Serialization;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddHostedService<ScheduledUpdateService>();
builder.Services.AddSingleton<IDatabaseConfiguration>(new DatabaseConfiguration(builder.Configuration["DatabasePath"]));
builder.Services.AddScoped<IAdminDataService, AdminDataService>();
builder.Services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
builder.Services.AddScoped<IUserRateLimitingService, UserRateLimitingService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserDataService, UserDataService>();
builder.Services.AddTransient<IClipboardService, ClipboardService>();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddBlazorise(options => { options.Immediate = true; });
builder.Services.AddBootstrapProviders();
builder.Services.AddFontAwesomeIcons();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseApiAuthentication();

app.UseClientRateLimiting();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using IServiceScope scope = app.Services.CreateScope();
using DatabaseContext databaseCcontext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
if (databaseCcontext.Database.EnsureCreated())
{
    logger.LogInformation("Automatically created database because it didn't exist.");
}

IClientPolicyStore clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
await clientPolicyStore.SeedAsync();

IUserRateLimitingService userRateLimitingService = scope.ServiceProvider.GetRequiredService<IUserRateLimitingService>();
await userRateLimitingService.LoadAllAsync();

app.Run();