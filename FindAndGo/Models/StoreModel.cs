using Newtonsoft.Json.Linq;

namespace FindAndGo.Models;

public class StoreModel
{
    private const string Chain = "Kroger";
    private const int RadiusInMiles = 10;
    private const int Limit = 10; // 10 is the default 
    public required string LocationId { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public required string Name { get; set; }
    public required string Latitude { get; set; }
    public required string Longitude { get; set; }


    public static async Task<IEnumerable<StoreModel>> GetLocations(HttpContext httpContext)
    {
        // Kroger API Reference: https://developer.kroger.com/reference#operation/SearchLocations

        var zipCode = httpContext.Request.Form["ZipCode"];
        var locationListUrl =
            $"https://api.kroger.com/v1/locations?filter.chain={Chain}&filter.zipCode.near={zipCode}&filter.radiusInMiles={RadiusInMiles}&filter.limit={Limit}";

        var token = httpContext.Request.Cookies["token"];

        var client = new HttpClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var getLocations = await client.GetStringAsync(locationListUrl);

        var resultsAsJson = JObject.Parse(getLocations)["data"].ToArray();

        var locations = new List<StoreModel>();

        foreach (var location in resultsAsJson)
        {
            var locationId = location["locationId"].ToString();
            var addressLine1 = location["address"]["addressLine1"].ToString();
            var city = location["address"]["city"].ToString();
            var state = location["address"]["state"].ToString();
            var zip = location["address"]["zipCode"].ToString();
            var name = location["name"].ToString();
            var longitude = location["geolocation"]["longitude"].ToString();
            var latitude = location["geolocation"]["latitude"].ToString();

            locations.Add(new StoreModel()
            {
                LocationId = locationId,
                Address = addressLine1,
                City = city,
                State = state,
                ZipCode = zip,
                Name = name,
                Longitude = longitude,
                Latitude = latitude
            });
        }

        return locations;
    }

    public static async Task<StoreModel> GetStoreDetails(HttpContext httpContext, string id)
    {
        var client = new HttpClient();
        var token = httpContext.Request.Cookies["token"];
        var locationDetailsUrl = $"https://api.kroger.com/v1/locations/{id}";
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var getLocations = await client.GetStringAsync(locationDetailsUrl);
        var resultsAsJson = JObject.Parse(getLocations)["data"];

        StoreModel storeModel = new StoreModel()
        {
            LocationId = resultsAsJson["locationId"].ToString(),
            Address = resultsAsJson["address"]["addressLine1"].ToString(),
            City = resultsAsJson["address"]["city"].ToString(),
            State = resultsAsJson["address"]["state"].ToString(),
            ZipCode = resultsAsJson["address"]["zipCode"].ToString(),
            Name = resultsAsJson["name"].ToString(),
            Longitude = resultsAsJson["geolocation"]["longitude"].ToString(),
            Latitude = resultsAsJson["geolocation"]["latitude"].ToString()
        };

        return storeModel;
    }
}