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
            try
            {
                var newTokenRequest = await new TokenService().GetAccessToken();
                TokenService.SetTokenAsCookie(newTokenRequest, HttpContext);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return Error();
            }
        }

        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}