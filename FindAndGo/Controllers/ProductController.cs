using FindAndGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class ProductController : Controller
{
    // GET
    public async Task<IActionResult> Index()
    {
        var products = await ProductModel.GetProducts(HttpContext);
        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> Search()
    {
        var locationId = HttpContext.Request.Form["locationId"];
        var searchTerm = HttpContext.Request.Form["searchTerm"];
        return Redirect($"/Product?locationId={locationId}&searchTerm={searchTerm}");
    }
}