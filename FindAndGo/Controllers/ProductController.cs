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
        var fullfillment = "ais"; // available in store

        var productSearchUrl =
            $"https://api-ce.kroger.com/v1/products?filter.term={searchTerm}&filter.locationId={locationId}&filter.fulfillment={fullfillment}";

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
                Brand = prod["brand"].ToString(),
                Description = prod["description"].ToString(),
                // The first item in the array is the featured imaged
                FeaturedImage = prod["images"][0]["sizes"][2]["url"].ToString(), // medium size
                Size = prod["items"][0]["size"].ToString()
            };

            var aisleLocations = prod["aisleLocations"];
            if (aisleLocations.Any())
            {
                var aisleLocation = new AisleLocation()
                {
                    BayNumber = aisleLocations[0]["bayNumber"].ToString(),
                    Description = aisleLocations[0]["description"].ToString(),
                    Side = aisleLocations[0]["side"].ToString(),
                    ShelfNumber = aisleLocations[0]["shelfNumber"].ToString()
                };

                product.AisleLocation = aisleLocation;
            }

            products.Add(product);
        }

        return View(products);
    }
}