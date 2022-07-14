namespace StatusExposed.Middleware;

public class ApiAuthenticationMiddleware
{
    private readonly RequestDelegate next;

    public ApiAuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Query.ContainsKey("test"))
        {
            context.Request.Headers.Add("X-ClientId", "test");
        }

        await next(context);
    }
}

public static class ApiAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseApiAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiAuthenticationMiddleware>();
    }
}