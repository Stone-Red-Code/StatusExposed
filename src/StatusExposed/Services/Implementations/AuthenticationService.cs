using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

using StatusExposed.Database;
using StatusExposed.Models;
using StatusExposed.Utilities;

namespace StatusExposed.Services.Implementations;

public class AuthenticationService : IAuthenticationService
{
    private readonly DatabaseContext mainDatabaseContext;
    private readonly ILogger<AuthenticationService> logger;
    private readonly NavigationManager navigationManager;
    private readonly ILocalStorageService localStorage;

    public AuthenticationService(DatabaseContext mainDatabaseContext, ILogger<AuthenticationService> logger, NavigationManager navigationManager, ILocalStorageService localStorage)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.logger = logger;
        this.navigationManager = navigationManager;
        this.localStorage = localStorage;
    }

    public async Task<User?> GetUserAsync()
    {
        string token = await localStorage.GetItemAsStringAsync("token");

        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        User? user = await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.SessionToken == token);

        if (user is null || DateTime.UtcNow - user.LastLoginDate > TimeSpan.FromDays(7))
        {
            return null;
        }

        return user;
    }

    public async Task RegisterUserAsync(string email)
    {
        string mailToken = "mail-" + SecureStringGenerator.CreateCryptographicRandomString(64);

        User user = new User(email)
        {
            LastLoginDate = DateTime.UtcNow,
            SessionToken = mailToken,
            IsVerified = false
        };

        // send mail
        logger.LogDebug("{email} | {mailToken}", email, navigationManager.ToAbsoluteUri($"/login/{mailToken}"));

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

            await localStorage.SetItemAsStringAsync("token", newToken);

            return true;
        }
    }

    public async Task LoginUserAsync(string email)
    {
        string mailToken = "mail-" + SecureStringGenerator.CreateCryptographicRandomString(64);

        User? user = await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return;
        }

        user.LastLoginDate = DateTime.UtcNow;
        user.SessionToken = mailToken;

        // send mail
        logger.LogDebug("{email} | {mailToken}", email, navigationManager.ToAbsoluteUri($"/login/{mailToken}"));

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
        string token = await localStorage.GetItemAsStringAsync("token");

        if (string.IsNullOrWhiteSpace(token))
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

        await localStorage.SetItemAsStringAsync("token", string.Empty);
    }

    public async Task<bool> IsAuthenticated()
    {
        string token = await localStorage.GetItemAsStringAsync("token");
        return await mainDatabaseContext.Users.FirstOrDefaultAsync(u => u.SessionToken == token) is not null;
    }
}