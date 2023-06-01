using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class StoreController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];
        if (token == null) return View("SessionExpired");

        try
        {
            var stores = await StoreModel.GetStores(HttpContext);
            return View(stores);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return View("PageNotFound");
        }
    }
}