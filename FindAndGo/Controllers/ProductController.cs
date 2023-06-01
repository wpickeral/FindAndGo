using System.Net;
using FindAndGo.Models;
using FindAndGo.Services;
using Microsoft.AspNetCore.Mvc;

namespace FindAndGo.Controllers;

public class ProductController : Controller
{
    // GET
    public async Task<IActionResult> Index()
    {
        var searchTerm = HttpContext.Request.Query["searchTerm"].ToString();
        var locationId = HttpContext.Request.Query["locationId"].ToString();
        var token = HttpContext.Request.Cookies["find-and-go.token"];
        if (token == null) return View("PageNotFound");

        try
        {
            // The request for a new token was successful, now we retry our products request with the new access token 
            var products = await ProductModel.GetProducts(searchTerm, locationId, token);
            return View(products);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
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