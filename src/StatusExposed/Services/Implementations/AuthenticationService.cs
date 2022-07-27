using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

using StatusExposed.Database;
using StatusExposed.Models;
using StatusExposed.Models.Options;
using StatusExposed.Utilities;

using System.Web;

namespace StatusExposed.Services.Implementations;

public class AuthenticationService : IAuthenticationService
{
    private readonly DatabaseContext mainDatabaseContext;
    private readonly ILogger<AuthenticationService> logger;
    private readonly NavigationManager navigationManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IJSRuntime jsRuntime;
    private readonly IEmailService emailService;
    private readonly EmailSettings mailOptions;

    public AuthenticationService(DatabaseContext mainDatabaseContext, ILogger<AuthenticationService> logger, NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor, IJSRuntime jsRuntime, IEmailService emailService, IOptions<EmailSettings> mailOptions)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
        this.navigationManager = navigationManager;
        this.httpContextAccessor = httpContextAccessor;
        this.jsRuntime = jsRuntime;
        this.emailService = emailService;
        this.mailOptions = mailOptions.Value;
    }

    public async Task<User?> GetUserAsync()
    {
        string? token = httpContextAccessor.HttpContext?.Request.Cookies["token"]?.ToString();

        if (!TokenGenerator.ValidateToken(token, "auth"))
        {
            return null;
        }

        User? user = await mainDatabaseContext.Users
            .Include(u => u.Permissions)
            .Include(u => u.ApiKeys)
            .AsSingleQuery()
            .FirstOrDefaultAsync(u => u.SessionToken == token);

        return user is null || user.IsBanned || DateTime.UtcNow - user.LastLoginDate > TimeSpan.FromDays(7) ? null : user;
    }

    public async Task RegisterUserAsync(string email)
    {
        string mailToken = TokenGenerator.GenerateToken("mail");

        User user = new User(email)
        {
            LastLoginDate = DateTime.UtcNow,
            SessionToken = mailToken,
            IsVerified = false
        };

        user.Permissions.Add("role:user");

        await SendVerificationEmail(email, mailToken);

        _ = mainDatabaseContext.Users.Add(user);
        _ = await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task<bool> VerifyUserAsync(string mailToken)
    {
        if (!TokenGenerator.ValidateToken(mailToken, "mail"))
        {
            return false;
        }

        User? user = await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.SessionToken == mailToken);

        if (user is null || DateTime.UtcNow - user.LastLoginDate > TimeSpan.FromMinutes(5))
        {
            return false;
        }
        else
        {
            string newToken = TokenGenerator.GenerateToken("auth", user.Id);

            user.LastLoginDate = DateTime.UtcNow;
            user.IsVerified = true;
            user.SessionToken = newToken;

            _ = await mainDatabaseContext.SaveChangesAsync();

            await WriteCookieAsync("token", newToken, 7);

            return true;
        }
    }

    public async Task<(bool Success, string? Message)> LoginUserAsync(string email)
    {
        User? user = await mainDatabaseContext.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return (false, null);
        }

        if (user.IsBanned)
        {
            string? banReason = user.Permissions.FirstOrDefault(p => p.Name.StartsWith("banreason:"))?.Name;

            return !string.IsNullOrWhiteSpace(banReason) ? ((bool Success, string? Message))(false, $"Ban reason: {banReason.Split(':')[1]}") : ((bool Success, string? Message))(false, null);
        }

        string mailToken = TokenGenerator.GenerateToken("mail", user.Id);

        user.LastLoginDate = DateTime.UtcNow;
        user.SessionToken = mailToken;

        await SendVerificationEmail(email, mailToken);

        _ = await mainDatabaseContext.SaveChangesAsync();

        return (true, null);
    }

    public async Task DeleteRequestUserAsync()
    {
        User? user = await GetUserAsync();

        if (user is null)
        {
            return;
        }

        string deletionToken = TokenGenerator.GenerateToken("delete", user.Id);

        await LogoutUserAsync();

        user.LastLoginDate = DateTime.UtcNow;
        user.SessionToken = deletionToken;

        await SendDeletionEmail(user.Email, deletionToken);

        _ = await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteUserAsync(string deletionToken)
    {
        if (!TokenGenerator.ValidateToken(deletionToken, "delete"))
        {
            return false;
        }

        User? user = await mainDatabaseContext.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.SessionToken == deletionToken);

        if (user is null)
        {
            return false;
        }

        user.Permissions.Clear();

        mainDatabaseContext.Subscriber.RemoveRange(mainDatabaseContext.Subscriber.Where(s => s.Email == user.Email));

        _ = await mainDatabaseContext.SaveChangesAsync();

        _ = mainDatabaseContext.Users.Remove(user);

        _ = await mainDatabaseContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await mainDatabaseContext.Users.AnyAsync(u => u.Email == email);
    }

    public async Task LogoutUserAsync()
    {
        string? token = httpContextAccessor.HttpContext?.Request.Cookies["token"]?.ToString();

        await WriteCookieAsync("token", string.Empty, 0);

        if (!TokenGenerator.ValidateToken(token, "auth"))
        {
            return;
        }

        User? user = await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.SessionToken == token);

        if (user is null)
        {
            return;
        }

        user.SessionToken = null;

        _ = await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task<bool> IsAuthenticated()
    {
        return await GetUserAsync() is not null;
    }

    private async Task WriteCookieAsync(string name, string value, int days)
    {
        _ = await jsRuntime.InvokeAsync<string>("blazorExtensions.WriteCookie", name, value, days);
    }

    private async Task SendVerificationEmail(string email, string mailToken)
    {
        string verificationLink = navigationManager.ToAbsoluteUri($"/login/{HttpUtility.UrlEncode(mailToken)}").ToString();

        if (File.Exists(mailOptions.TemplatePaths?.AccountVerification))
        {
            await emailService.SendWithTemeplateAsync(email, "Account Verification", mailOptions.TemplatePaths.AccountVerification, templateParameters: new TemplateParameter("verification-link", verificationLink));
        }
        else
        {
            logger.LogWarning("Account verification E-mail template not found, using fall back template.");
            await emailService.SendAsync(email, "Account Verification", $"Verify your account: <a href={verificationLink}>verify</a>");
        }
    }

    private async Task SendDeletionEmail(string email, string deletionToken)
    {
        string verificationLink = navigationManager.ToAbsoluteUri($"/logout/{HttpUtility.UrlEncode(deletionToken)}").ToString();

        if (File.Exists(mailOptions.TemplatePaths?.AccountDeletion))
        {
            await emailService.SendWithTemeplateAsync(email, "Account Deletion", mailOptions.TemplatePaths.AccountDeletion, templateParameters: new TemplateParameter("deletion-link", verificationLink));
        }
        else
        {
            logger.LogWarning("Account deletion E-mail template not found, using fall back template.");
            await emailService.SendAsync(email, "Account Deletion", $"Delete your account: <a href={verificationLink}>delete</a>");
        }
    }
}