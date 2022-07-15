﻿using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IAuthenticationService
{
    Task<User?> GetUserAsync();

    Task RegisterUserAsync(string email);

    Task LoginUserAsync(string email);

    Task LogoutUserAsync();

    Task<bool> VerifyUserAsync(string mailToken);

    Task DeleteUserAsync();

    Task<bool> UserExistsAsync(string email);

    Task<bool> IsAuthenticated();
}