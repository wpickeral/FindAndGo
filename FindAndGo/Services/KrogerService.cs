using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Services;

public class KrogerService : IKrogerService
{
    private IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private static readonly string _baseUrl = "https://api.kroger.com/v1/";

    public KrogerService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<JObject?> GetAccessToken()
    {
        var clientId = _configuration["Kroger:ClientId"];
        var clientSecret = _configuration["Kroger:ClientSecret"];
        const string grant = "client_credentials";
        var url = $"{_baseUrl}connect/oauth2/token";

        // Create the auth string according to Kroger API requirements:
        // https://developer.kroger.com/reference#operation/authorizationCode
        var authString = $"{clientId}:{clientSecret}";
        // Base 64 encode the auth string
        var authStringEncoded = StringToBase64String(authString);
        // Add the authorization header 
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authStringEncoded}");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Create the scopes object to include with the post request
        var body = new List<KeyValuePair<string, string>>();
        var grantType = new KeyValuePair<string, string>("grant_type", grant);
        var scope = new KeyValuePair<string, string>("scope", "product.compact"); // provides access to /product
        // No scope required to access /location
        body.Add(grantType);
        body.Add(scope);

        var getAccessToken = await _httpClient.PostAsync(url, new FormUrlEncodedContent(body));

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

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var getProducts = await _httpClient.GetStringAsync(_httpClient.BaseAddress.AbsoluteUri);

        return JObject.Parse(getProducts)["data"];
    }


    public async Task<JToken> GetStores(string chain, int zipCode, int radiusInMiles, int limit,
        string token)
    {
        var url =
            $"{_baseUrl}locations?filter.chain={chain}&filter.zipCode.near={zipCode}&filter.radiusInMiles={radiusInMiles}&filter.limit={limit}";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var getStores = await _httpClient.GetStringAsync(url);

        return JObject.Parse(getStores)["data"];
    }

    public async Task<JToken> GetStoreDetails(string id, string token)
    {
        var url = $"{_baseUrl}locations/{id}";
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var getStoreDetails = await _httpClient.GetStringAsync(url);
        return JObject.Parse(getStoreDetails)["data"];
    }


    private static string StringToBase64String(string str)
    {
        var authStringTextBytes = System.Text.Encoding.UTF8.GetBytes(str);

        return Convert.ToBase64String(authStringTextBytes);
    }
}