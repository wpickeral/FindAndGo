using Newtonsoft.Json.Linq;

namespace FindAndGo.Services;

public interface ITokenService
{
    public Task<JObject?> GetAccessToken();
}