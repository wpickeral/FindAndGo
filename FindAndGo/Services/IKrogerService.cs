using FindAndGo.Models;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Services;

public interface IKrogerService
{
    public Task<JObject?> GetAccessToken();
    public Task<JToken> GetProducts(string searchTerm, string locationId, string token);
    public Task<JToken> GetStores(string chain, int zipCode, int radiusInMiles, int limit, string token);
    public Task<JToken> GetStoreDetails(string id, string token);
}