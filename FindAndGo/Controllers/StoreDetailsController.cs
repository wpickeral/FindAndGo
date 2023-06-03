using FindAndGo.Models;
using FindAndGo.Services;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class StoreDetailsController : Controller
{
    private IKrogerService _krogerService;

    public StoreDetailsController(IKrogerService krogerService) => _krogerService = krogerService;

    [HttpGet]
    public async Task<IActionResult> Index(string id)
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];

        if (token == null) // Try to get a new token
        {
            try
            {
                var newTokenRequest = await _krogerService.GetAccessToken();
                if (newTokenRequest != null) ControllerHelpers.AddTokenAsCookieToResponse(newTokenRequest, HttpContext);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return View("PageNotFound");
            }
        }

        var storeDetails = await StoreModel.GetStoreDetails(HttpContext, id);

        return View(storeDetails);
    }
}