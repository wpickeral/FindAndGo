using FindAndGo.Services;

namespace FindAndGo.Middleware;

public class KrogerMiddleware
{
    private readonly RequestDelegate _next;

    public KrogerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // IKrogerService is injected into InvokeAsync
    public async Task InvokeAsync(HttpContext httpContext, IKrogerService krogerService)
    {
        var token = httpContext.Request.Cookies["find-and-go.token"];

        if (token == null)
        {
            try
            {
                // Request a new token
                var newTokenRequest = await krogerService.GetAccessToken();
                if (newTokenRequest != null)
                {
                    // Parse the access_token
                    var newToken = newTokenRequest["access_token"].ToString();
                    // Parse the MaxAge
                    var expiresIn = int.Parse(newTokenRequest["expires_in"].ToString());
                    // Build the cookie options object
                    var cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTimeOffset.Now.AddSeconds(expiresIn);
                    // Add the token to the cookies object with the options
                    httpContext.Response.Cookies.Append("find-and-go.token", newToken, cookieOptions);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }

        await _next(httpContext);   
        
        Console.WriteLine("One the way back");
    }
}