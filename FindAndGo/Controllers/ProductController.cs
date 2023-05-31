using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class ProductController : Controller
{
    // GET
    public async Task<IActionResult> Index()
    {
        try
        {
            var products = await ProductModel.GetProducts(HttpContext);
            return View(products);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            var locationId = HttpContext.Request.Query["locationId"];
            var searchTerm = HttpContext.Request.Query["searchTerm"];

            return Redirect($"/Product/NoResultsFound?locationId={locationId}&searchTerm={searchTerm}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return View("PageNotFound");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Search()
    {
        var locationId = HttpContext.Request.Form["locationId"];
        var searchTerm = HttpContext.Request.Form["searchTerm"];

        return Redirect($"/Product?locationId={locationId}&searchTerm={searchTerm}");
    }

    [HttpGet]
    public IActionResult NoResultsFound([FromQuery] string locationId, string searchTerm)
    {
        ViewBag.LocationId = locationId;
        ViewBag.SearchTerm = searchTerm;

        return View();
    }
}