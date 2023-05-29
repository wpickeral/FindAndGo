using Newtonsoft.Json.Linq;

namespace FindAndGo.Models;

public class ProductModel
{
    public string ProductId { get; set; }
    public string Brand { get; set; }
    public string Description { get; set; }
    public string FeaturedImage { get; set; }
    public string Size { get; set; }

    public AisleLocation? AisleLocation { get; set; }
}