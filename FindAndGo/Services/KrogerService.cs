using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Services;

public class KrogerService : IKrogerService
{
    private IHttpClientFactory _httpClientFactory;
    private HttpClient _httpClient;
    private static readonly string _baseUrl = "https://api.kroger.com/v1/";

    public KrogerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JObject?> GetAccessToken()
    {
        var client = new HttpClient();
        var clientId = Environment.GetEnvironmentVariable("KROGER_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("KROGER_CLIENT_SECRET");
        const string grant = "client_credentials";
        var url = $"{_baseUrl}connect/oauth2/token";

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

        var getAccessToken = await client.PostAsync(url, new FormUrlEncodedContent(body));

        if (getAccessToken.IsSuccessStatusCode)
        {
            var results = getAccessToken.Content.ReadAsStringAsync().Result;
            var tokenResponse = JObject.Parse(results);
            return tokenResponse;
        }

        return null;
    }


    public async Task<JToken> GetProducts(string searchTerm, string locationId, string token)
    {
        var path =
            $"products?filter.term={searchTerm}&filter.locationId={locationId}&filter.fulfillment=ais";
        _httpClient.BaseAddress = new Uri($"{_baseUrl}{path}");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var getStores = await _httpClient.GetStringAsync(_httpClient.BaseAddress.AbsoluteUri);
        var resultsAsJson = JObject.Parse(getStores)["data"];
        return resultsAsJson;
    }


    public async Task<JToken> GetStores(string chain, int zipCode, int radiusInMiles, int limit,
        string token)
    {
        // Kroger API Reference: https://developer.kroger.com/reference#operation/SearchLocations

        var url =
            $"{_baseUrl}locations?filter.chain={chain}&filter.zipCode.near={zipCode}&filter.radiusInMiles={radiusInMiles}&filter.limit={limit}";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var GetStores = await client.GetStringAsync(url);

        return JObject.Parse(GetStores)["data"];
    }


    private static string StringToBase64String(string str)
    {
        var authStringTextBytes = System.Text.Encoding.UTF8.GetBytes(str);

        return Convert.ToBase64String(authStringTextBytes);
    }
}