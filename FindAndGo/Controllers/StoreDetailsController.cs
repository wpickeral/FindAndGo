using FindAndGo.Models;
using FindAndGo.Services;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class StoreDetailsController : Controller
{
    private IKrogerService _krogerService;

    public StoreDetailsController(IKrogerService krogerService)
    {
        _krogerService = krogerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string id)
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];
        if (token == null) return View("PageNotFound");

        try
        {
            var storeDetailsRequest = await _krogerService.GetStoreDetails(id, token);
            var storeDetails = StoreModel.StoreDetails(storeDetailsRequest);
           
            return View(storeDetails);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return View("PageNotFound");
        }
    }
}