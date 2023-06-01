using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class StoreDetailsController : Controller
{
    // GET
    public async Task<IActionResult> Index(string id)
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];
        if (token == null) return View("SessionExpired");

        var storeDetails = await StoreModel.GetStoreDetails(HttpContext, id);

        return View(storeDetails);
    }
}