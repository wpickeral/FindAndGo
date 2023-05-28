using Newtonsoft.Json.Linq;

namespace FindAndGo.Models;

public class ProductModel
{
    public string ProductId { get; set; }
    public JToken AisleLocations { get; set; }
    public string Brand { get; set; }
    public string Description { get; set; }
    public string FeaturedImage { get; set; }
}