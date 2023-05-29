using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Controllers;

public class StoreDetailsController : Controller
{
    // GET
    public async Task<IActionResult> Index(string id)
    {
        var client = new HttpClient();
        var token = Request.Cookies["token"];
        var locationDetailsUrl = $"https://api-ce.kroger.com/v1/locations/{id}";
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

        return View(storeModel);
    }

    [HttpPost]
    public async Task<IActionResult> Search()
    {
        var locationId = HttpContext.Request.Form["locationId"];
        var searchTerm = HttpContext.Request.Form["searchTerm"];
        return Redirect($"/Product?locationId={locationId}&searchTerm={searchTerm}");
    }
}