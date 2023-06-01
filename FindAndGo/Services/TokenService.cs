using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using FindAndGo.Models;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Services;

public class TokenService : ITokenService
{
    private const string AccessTokenUrl = "https://api.kroger.com/v1/connect/oauth2/token";
    private const string RefreshTokenUrl = "";

    public async Task<JObject> GetAccessToken(Boolean refresh = false)
    {
        var client = new HttpClient();
        var clientId = Environment.GetEnvironmentVariable("KROGER_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("KROGER_CLIENT_SECRET");

        var grant = refresh ? "refresh_token" : "client_credentials";

        // Create the auth string according to Kroger API requirements:
        // https://developer.kroger.com/reference#operation/authorizationCode
        var authString = $"{clientId}:{clientSecret}";
        // Base 64 encode the auth string
        var authStringEncoded = StringToBase64String(authString);
        // Add the authorization header 
        client.DefaultRequestHeaders.Add("Authorization", $"Basic {authStringEncoded}");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Create the scopes object to include with the post request
        var body = new List<KeyValuePair<string, string>>();
        var grantType = new KeyValuePair<string, string>("grant_type", grant);
        var scope = new KeyValuePair<string, string>("scope", "product.compact"); // provides access to /product
        // No scope required to access /location
        body.Add(grantType);
        body.Add(scope);

        var getAccessToken = await client.PostAsync(AccessTokenUrl, new FormUrlEncodedContent(body));

        if (getAccessToken.IsSuccessStatusCode)
        {
            var results = getAccessToken.Content.ReadAsStringAsync().Result;
            var tokenResponse = JObject.Parse(results);
            return tokenResponse;
        }

        return new JObject();
    }

    public static CookieOptions BuildCookieOptions(int expires_in)
    {
        var cookieOptions = new CookieOptions();
        cookieOptions.Expires = DateTimeOffset.Now.AddSeconds(expires_in);

        return cookieOptions;
    }

    private static string StringToBase64String(string str)
    {
        var authStringTextBytes = System.Text.Encoding.UTF8.GetBytes(str);

        return Convert.ToBase64String(authStringTextBytes);
    }
}