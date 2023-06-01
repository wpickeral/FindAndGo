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
        IEnumerable<ProductModel> products;

        try
        {
            products = await ProductModel.GetProducts(searchTerm, locationId, token);
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
                    try
                    {
                        // Request a new token
                        var newTokenRequest = await new TokenService().GetAccessToken();
                        var newToken = newTokenRequest["access_token"].ToString();
                        // Parse the MaxAge
                        var expiresIn = int.Parse(newTokenRequest["expires_in"].ToString());
                        // Build the cookie options object
                        var cookieOptions = TokenService.BuildCookieOptions(expiresIn);
                        // Added the token to the cookies with the options
                        HttpContext.Response.Cookies.Append("find-and-go.token", newToken.ToString(), cookieOptions);

                        // If the new access token request is successful 
                        products = await ProductModel.GetProducts(searchTerm, locationId, newToken);
                        return View(products);
                    }
                    catch (HttpRequestException Error)
                    {
                        Console.WriteLine(Error);
                        return View("PageNotFound");
                    }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

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