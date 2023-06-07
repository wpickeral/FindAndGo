using FindAndGo.Models;
using FindAndGo.Services;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class StoreController : Controller
{
    private IKrogerService _krogerService;

    public StoreController(IKrogerService krogerService) => _krogerService = krogerService;

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];
        if (token == null) return View("PageNotFound");

        try
        {
            var chain = ""; // Include all Kroger owned stores
            var radiusInMiles = 10;
            var limit = 10; // 10 is the default 
            int.TryParse(HttpContext.Request.Form["ZipCode"], out var zipCode);
            var storesRequest = await _krogerService.GetStores(chain, zipCode, radiusInMiles, limit, token);
            var stores = StoreModel.Stores(storesRequest);

            return View(stores);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return View("PageNotFound");
        }
    }
}