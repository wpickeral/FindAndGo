using Newtonsoft.Json.Linq;

namespace FindAndGo.Models;

public interface ITokenService
{
    public Task<JToken?> GetAccessToken();
}