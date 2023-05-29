namespace FindAndGo.Models;

public class StoreModel
{
    public required string LocationId { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }
    public required string Name { get; set; }
    public required string Latitude { get; set; }
    public required string Longitude { get; set; }
}