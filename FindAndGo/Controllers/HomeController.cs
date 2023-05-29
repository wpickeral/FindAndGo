using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FindAndGo.Models;
using Newtonsoft.Json.Linq;


namespace FindAndGo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private static JToken? AccessToken { get; set; }
    private static JToken? RefreshToken { get; set; }

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        // If the access token is null we get a new token and and set it to the AccessToken
        AccessToken ??= await new Auth().GetAccessToken();

        Response.Cookies.Append("token", AccessToken.ToString());
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}