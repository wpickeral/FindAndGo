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


    public static async Task<IEnumerable<ProductModel>> GetProducts(string searchTerm, string locationId, string token)
    {
        //  https://developer.kroger.com/reference#operation/productGet

        const string fulfillment = "ais"; // available in store

        var productSearchUrl =
            $"https://api.kroger.com/v1/products?filter.term={searchTerm}&filter.locationId={locationId}&filter.fulfillment={fulfillment}";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var GetStores = await client.GetStringAsync(productSearchUrl);
        var resultsAsJson = JObject.Parse(GetStores)["data"];

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