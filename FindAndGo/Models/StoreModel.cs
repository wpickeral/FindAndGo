using Newtonsoft.Json.Linq;

namespace FindAndGo.Models;

public class StoreModel
{
    public required string LocationId { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public required string Name { get; set; }
    public required string Latitude { get; set; }
    public required string Longitude { get; set; }

    public static IEnumerable<StoreModel> Stores(JToken resultsAsJson)
    {
        // Kroger API Reference: https://developer.kroger.com/reference#operation/SearchLocations

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

    public static StoreModel StoreDetails(JToken resultsAsJson)
    {
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