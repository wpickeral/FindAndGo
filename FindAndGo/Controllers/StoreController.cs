using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Controllers;

public class StoreController : Controller
{
    private const string Chain = "Kroger";
    private const int RadiusInMiles = 10;
    private const int Limit = 10; // 10 is the default

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        // Kroger API Reference: https://developer.kroger.com/reference#operation/SearchLocations

        var zipCode = HttpContext.Request.Form["ZipCode"];
        var locationListUrl =
            $"https://api-ce.kroger.com/v1/locations?filter.chain={Chain}&filter.zipCode.near={zipCode}&filter.radiusInMiles={RadiusInMiles}&filter.limit={Limit}";

        var token = Request.Cookies["token"];

        var client = new HttpClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var getLocations = await client.GetStringAsync(locationListUrl);

        var resultsAsJson = JObject.Parse(getLocations)["data"].ToArray();

        var locations = new List<LocationModel>();

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

            locations.Add(new LocationModel()
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

        return View(locations);
    }
}