using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FindAndGo.Controllers;

public class StoreDetailsController : Controller
{
    // GET
    public async Task<IActionResult> Index(string id)
    {
        var storeDetails = await StoreModel.GetStoreDetails(HttpContext, id);

        return View(storeDetails);
    }
}