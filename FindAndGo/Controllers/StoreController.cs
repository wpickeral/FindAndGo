using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class StoreController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var stores = await StoreModel.GetStores(HttpContext);
        return View(stores);
    }
}