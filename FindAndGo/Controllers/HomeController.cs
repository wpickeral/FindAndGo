using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FindAndGo.Models;
using FindAndGo.Services;

namespace FindAndGo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];

        // No token available
        if (token == null)
        {
            var newTokenRequest = await new TokenService().GetAccessToken();
            var newToken = newTokenRequest["access_token"];
            var newTokenExpiresIn = int.Parse(newTokenRequest["expires_in"].ToString());

            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTimeOffset.Now.AddSeconds(newTokenExpiresIn);

            HttpContext.Response.Cookies.Append("find-and-go.token", newToken.ToString(), cookieOptions);
        }

        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}