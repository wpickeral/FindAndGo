using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Controllers;

public class ProductController : Controller
{
    // GET
    public async Task<IActionResult> Index()
    {
        //  https://developer.kroger.com/reference#operation/productGet

        var searchTerm = HttpContext.Request.Query["searchTerm"];
        var locationId = HttpContext.Request.Query["locationId"];
        const string fulfillment = "ais"; // available in store

        var productSearchUrl =
            $"https://api-ce.kroger.com/v1/products?filter.term={searchTerm}&filter.locationId={locationId}&filter.fulfillment={fulfillment}";

        var client = new HttpClient();
        var token = Request.Cookies["token"];
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var getLocations = await client.GetStringAsync(productSearchUrl);
        var resultsAsJson = JObject.Parse(getLocations)["data"];

        var products = new List<ProductModel>();

        Console.WriteLine(resultsAsJson);

        foreach (var prod in resultsAsJson)
        {
            var product = new ProductModel
            {
                ProductId = prod["productId"].ToString(),
                Description = prod["description"].ToString(),
                // The first item in the array is the featured imaged
                FeaturedImage = prod["images"][0]["sizes"][1]["url"].ToString(), // large size
                Size = prod["items"][0]["size"].ToString()
            };

            var aisleLocations = prod["aisleLocations"];
            if (aisleLocations.Any())
            {
                var aisleLocation = new AisleLocation()
                {
                    Description = aisleLocations[0]["description"].ToString(),
                };

                product.AisleLocation = aisleLocation;
            }

            products.Add(product);
        }

        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> Search()
    {
        var locationId = HttpContext.Request.Form["locationId"];
        var searchTerm = HttpContext.Request.Form["searchTerm"];
        return Redirect($"/Product?locationId={locationId}&searchTerm={searchTerm}");
    }
}