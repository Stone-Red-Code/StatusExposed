using StatusExposed.Models;
using StatusExposed.Services;

namespace StatusExposed.Middleware;

public class ApiAuthenticationMiddleware
{
    private readonly RequestDelegate next;

    public ApiAuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService, ILogger<ApiAuthenticationMiddleware> logger)
    {
        if (context.Request.Headers.ContainsKey("X-ClientId"))
        {
            context.Response.Headers.Remove("X-ClientId");
        }

        if (await authenticationService.IsAuthenticated())
        {
            User user = (await authenticationService.GetUserAsync())!;

            Permission? permission = user.Permissions.FirstOrDefault(p => p.Name.StartsWith("api"));

            if (permission is not null)
            {
                context.Request.Headers.Add("X-ClientId", permission.Name);
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