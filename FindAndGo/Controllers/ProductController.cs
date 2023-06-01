using System.Net;
using FindAndGo.Models;
using FindAndGo.Services;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class ProductController : Controller
{
    // GE
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string locationId, string searchTerm)
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];
        if (token == null) return View("SessionExpired");

        try
        {
            // The request for a new token was successful, now we retry our products request with the new access token 
            var products = await ProductModel.GetProducts(searchTerm, locationId, token);
            return View(products);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            var routeValues = new List<KeyValuePair<string, string>>
            {
                new("searchTerm", searchTerm),
                new("locationId", locationId),
            };

            return RedirectToAction("NoResultsFound", routeValues);
        }
    }

    [HttpPost]
    public IActionResult Search([FromForm] string locationId, string searchTerm)
    {
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