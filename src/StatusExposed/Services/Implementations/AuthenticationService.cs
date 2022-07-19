﻿using Microsoft.AspNetCore.Components;
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
    private readonly EmailOptions mailOptions;

    public AuthenticationService(DatabaseContext mainDatabaseContext, ILogger<AuthenticationService> logger, NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor, IJSRuntime jsRuntime, IEmailService emailService, IOptions<EmailOptions> mailOptions)
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

        if (!IsValidToken(token))
        {
            return null;
        }

        User? user = await mainDatabaseContext.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.SessionToken == token);

        if (user is null || DateTime.UtcNow - user.LastLoginDate > TimeSpan.FromDays(7))
        {
            return null;
        }

        return user;
    }

    public async Task RegisterUserAsync(string email)
    {
        string mailToken = GenerateMailToken();

        User user = new User(email)
        {
            LastLoginDate = DateTime.UtcNow,
            SessionToken = mailToken,
            IsVerified = false
        };

        SendVerificationEmail(email, mailToken);

        mainDatabaseContext.Users.Add(user);
        await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task<bool> VerifyUserAsync(string mailToken)
    {
        if (!mailToken.StartsWith("mail-"))
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
            string newToken = SecureStringGenerator.CreateCryptographicRandomString(128);

            user.LastLoginDate = DateTime.UtcNow;
            user.IsVerified = true;
            user.SessionToken = newToken;

            await mainDatabaseContext.SaveChangesAsync();

            await WriteCookieAsync("token", newToken, 7);

            return true;
        }
    }

    public async Task LoginUserAsync(string email)
    {
        string mailToken = GenerateMailToken();

        User? user = await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return;
        }

        user.LastLoginDate = DateTime.UtcNow;
        user.SessionToken = mailToken;

        SendVerificationEmail(email, mailToken);

        await mainDatabaseContext.SaveChangesAsync();
    }

    public Task DeleteUserAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await mainDatabaseContext.Users.AnyAsync(u => u.Email == email);
    }

    public async Task LogoutUserAsync()
    {
        string? token = httpContextAccessor.HttpContext?.Request.Cookies["token"]?.ToString();

        if (!IsValidToken(token))
        {
            return;
        }

        User? user = await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.SessionToken == token);

        if (user is null)
        {
            return;
        }

        user.SessionToken = null;

        await mainDatabaseContext.SaveChangesAsync();

        await WriteCookieAsync("token", string.Empty, 0);
    }

    public async Task<bool> IsAuthenticated()
    {
        return await GetUserAsync() is not null;
    }

    private async Task WriteCookieAsync(string name, string value, int days)
    {
        _ = await jsRuntime.InvokeAsync<string>("blazorExtensions.WriteCookie", name, value, days);
    }

    private static string GenerateMailToken()
    {
        return "mail-" + SecureStringGenerator.CreateCryptographicRandomString(64);
    }

    private static bool IsValidToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token) || token.StartsWith("mail-"))
        {
            return false;
        }

        return true;
    }

    private void SendVerificationEmail(string email, string mailToken)
    {
        string verificationLink = navigationManager.ToAbsoluteUri($"/login/{HttpUtility.UrlEncode(mailToken)}").ToString();

        if (File.Exists(mailOptions.TemplatePaths?.AccountVerification))
        {
            emailService.SendWithTemeplate(email, "Account Verification", mailOptions.TemplatePaths.AccountVerification, templateParameters: new TemplateParameter("verification-link", verificationLink));
        }
        else
        {
            logger.LogWarning("Account verification E-Mail template not found, using fall back template.");
            emailService.Send(email, "Account Verification", $"Verify your account: <a href={verificationLink}>verify</a>");
        }
    }
}