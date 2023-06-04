using System.Globalization;
using FindAndGo.Services;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Models;

public class ProductModel
{
    public required string ProductId { get; init; }
    public required string LocationId { get; init; }
    public required string Description { get; init; }
    public required string FeaturedImage { get; init; }
    public required string Size { get; init; }
    public decimal RegularPrice { get; set; }
    public decimal PromoPrice { get; set; }

    public AisleLocation? AisleLocation { get; set; }


    public static IEnumerable<ProductModel?> Products(JToken resultsAsJson, string locationId)
    {
        //  https://developer.kroger.com/reference#operation/productGet

        var products = new List<ProductModel>();

        foreach (var prod in resultsAsJson)
        {
            var product = new ProductModel
            {
                LocationId = locationId,
                ProductId = prod["productId"].ToString(),
                Description = prod["description"].ToString(),
                // The first item in the array is the featured imaged
                FeaturedImage = prod["images"][0]["sizes"][1]["url"].ToString(), // large size
                Size = prod["items"][0]["size"].ToString(),
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

            var price = prod["items"][0]["price"];
            if (price != null)
            {
                product.RegularPrice = decimal.Parse(price["regular"].ToString());
                product.PromoPrice = decimal.Parse(price["promo"].ToString());
            }

            products.Add(product);
        }

        return products;
    }
}