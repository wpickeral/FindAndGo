using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Controllers;

public class ProductController : Controller
{
    // GET
    public async Task<IActionResult> Index()
    {
        var searchTerm = HttpContext.Request.Query["searchTerm"];
        var locationId = HttpContext.Request.Query["locationId"];

        var productSearchUrl =
            $"https://api-ce.kroger.com/v1/products?filter.term={searchTerm}&filter.locationId={locationId}";

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
            products.Add(new ProductModel
            {
                ProductId = prod["productId"].ToString(),
                AisleLocations = prod["aisleLocations"],
                Brand = prod["brand"].ToString(),
                Description = prod["description"].ToString(),
                // The first item in the array is the featured imaged
                FeaturedImage = prod["images"][0]["sizes"][2]["url"].ToString() // medium size
            });
        }

        return View(products);
    }
}