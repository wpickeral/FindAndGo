namespace FindAndGo.Models;

public class ProductModel
{
    public required string ProductId { get; init; }
    public required string Description { get; init; }
    public required string FeaturedImage { get; init; }
    public required string Size { get; init; }

    public AisleLocation? AisleLocation { get; set; }
}