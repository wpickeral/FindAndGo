using Newtonsoft.Json.Linq;

namespace FindAndGo.Controllers;

public class ControllerHelpers
{
    public static void AddTokenAsCookieToResponse(JToken newTokenRequest, HttpContext httpContext)
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