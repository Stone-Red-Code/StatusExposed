﻿using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAdminDataService
{
    Task<User?> GetUserInfoAsync(string email);

    Task LogOutUserAsync(string email);

    Task<int> GetAmountOfUsersAsync(bool? verified = null);

    Task<int> GetAmountOfServicesAsync();
}