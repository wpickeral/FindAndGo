namespace FindAndGo.Middleware;

public static class KrogerMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestKrogerAccessToken(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<KrogerMiddleware>();
    }
}