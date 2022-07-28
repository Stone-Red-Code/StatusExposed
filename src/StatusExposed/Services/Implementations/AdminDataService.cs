﻿using Microsoft.EntityFrameworkCore;

using StatusExposed.Database;
using StatusExposed.Models;

namespace StatusExposed.Services.Implementations;

public class AdminDataService : IAdminDataService
{
    private readonly DatabaseContext mainDatabaseContext;
    private readonly IAuthorizationService authorizationService;
    private readonly ILogger<AdminDataService> logger;

    public AdminDataService(DatabaseContext mainDatabaseContext, IAuthorizationService authorizationService, ILogger<AdminDataService> logger)
    {
        this.mainDatabaseContext = mainDatabaseContext;
        this.authorizationService = authorizationService;
        this.logger = logger;
    }

    public async Task<int> GetAmountOfServicesAsync()
    {
        return !await authorizationService.IsAuthorized("role:admin") ? -1 : mainDatabaseContext.Services.Count();
    }

    public async Task<int> GetAmountOfUsersAsync(bool? verified = null)
    {
        if (verified is null)
        {
            return await mainDatabaseContext.Users.CountAsync();
        }
        else
        {
            return await mainDatabaseContext.Users.Where(u => u.IsVerified == verified).CountAsync();
        }
    }

    public async Task<User?> GetUserInfoAsync(string email)
    {
        email = email.Trim().ToLower();

        if (!await authorizationService.IsAuthorized("role:admin"))
        {
            return null;
        }

        logger.LogInformation("An admin requested the data from {email}", email);

        return await mainDatabaseContext.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task DeleteUserAsync(string email)
    {
        email = email.Trim().ToLower();

        if (!await authorizationService.IsAuthorized("role:admin"))
        {
            return;
        }

        User? user = await mainDatabaseContext.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return;
        }

        mainDatabaseContext.Subscriber.RemoveRange(mainDatabaseContext.Subscriber.Where(s => s.Email == user.Email));

        _ = mainDatabaseContext.Users.Remove(user);

        _ = await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task AddPermissionToUserAsync(string email, Permission permission)
    {
        email = email.Trim().ToLower();

        if (!await authorizationService.IsAuthorized("role:admin"))
        {
            return;
        }

        User? user = await GetUserInfoAsync(email);

        if (user is null)
        {
            return;
        }

        logger.LogInformation("An admin added the permission ({permission}) to the user {email}", permission.Name, email);

        user.Permissions.Add(permission);

        _ = await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task RemovePermissionFromUserAsync(string email, Permission permission)
    {
        email = email.Trim().ToLower();

        if (!await authorizationService.IsAuthorized("role:admin"))
        {
            return;
        }

        User? user = await GetUserInfoAsync(email);

        if (user is null)
        {
            return;
        }

        logger.LogInformation("An admin removed the permission ({permission}) to the user {email}", permission.Name, email);

        _ = user.Permissions.Remove(permission);

        _ = await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task SetUserBan(string email, bool isBanned)
    {
        email = email.Trim().ToLower();

        if (!await authorizationService.IsAuthorized("role:admin"))
        {
            return;
        }

        User? user = await GetUserInfoAsync(email);

        if (user is null)
        {
            return;
        }

        logger.LogInformation("An admin {banned} the user {email}", isBanned ? "banned" : "unbanned", email);

        user.IsBanned = isBanned;

        _ = await mainDatabaseContext.SaveChangesAsync();
    }

    public async Task PurgeUnverifiedUsersAsync()
    {
        IQueryable<User>? entriesToDelete = mainDatabaseContext.Users.Include(u => u.Permissions).Where(u => !u.IsVerified);

        logger.LogInformation("An admin purged {count} unverified users", await entriesToDelete.CountAsync());

        mainDatabaseContext.Users.RemoveRange(entriesToDelete);

        _ = await mainDatabaseContext.SaveChangesAsync();
    }
}