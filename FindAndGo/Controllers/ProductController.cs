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

        try
        {
            var token = HttpContext.Request.Cookies["find-and-go.token"];
            var products = await ProductModel.GetProducts(searchTerm, locationId, token);
            return View(products);
        }
        // Initiate the retry logic 
        catch (HttpRequestException e)
        {
            switch (e.StatusCode)
            {
                // No results found
                case HttpStatusCode.BadRequest:
                    return Redirect($"/Product/NoResultsFound?locationId={locationId}&searchTerm={searchTerm}");
                case HttpStatusCode.Unauthorized:
                    // Access token is expired
                    // Request a new token and update the cookie
                    string newToken;
                    try
                    {
                        // Request a new token
                        var newTokenRequest = await new TokenService().GetAccessToken();
                        TokenService.SetTokenAsCookie(newTokenRequest, HttpContext);
                        // Assign the access token to newToken so it can be used in the products request below
                        newToken = newTokenRequest["access_token"].ToString();
                    }
                    catch (HttpRequestException error)
                    {
                        Console.WriteLine(error);
                        return View("PageNotFound");
                    }

                    try
                    {
                        // The request for a new token was successful, now we retry our products request with the new access token 
                        var products = await ProductModel.GetProducts(searchTerm, locationId, newToken);
                        return View(products);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        return View("PageNotFound");
                    }
            }
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
        }

        // Something else went wrong
        return View("PageNotFound");
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