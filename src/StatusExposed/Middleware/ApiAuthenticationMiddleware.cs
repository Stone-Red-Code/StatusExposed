using Microsoft.EntityFrameworkCore;

using StatusExposed.Database;
using StatusExposed.Models;

namespace StatusExposed.Middleware;

public class ApiAuthenticationMiddleware
{
    private readonly RequestDelegate next;

    public ApiAuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, DatabaseContext mainDatabaseContext)
    {
        if (context.Request.Headers.ContainsKey("X-ClientId"))
        {
            context.Request.Headers.Remove("X-ClientId");
        }

        string? authorizationText = context.Request.Headers.Authorization.FirstOrDefault();

        if (authorizationText is not null)
        {
            string[] parts = authorizationText.Split(' ');

            User? user = await mainDatabaseContext.Users
                 .Include(u => u.ApiKeys)
                 .FirstOrDefaultAsync(u => u.ApiKeys
                 .Any(a => a.Key == parts.Last()));

            if (user is not null)
            {
                context.Request.Headers.Add("X-ClientId", user.Id.ToString());
            }
        }

        await next(context);
    }
}

public static class ApiAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseApiAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiAuthenticationMiddleware>();
    }
}