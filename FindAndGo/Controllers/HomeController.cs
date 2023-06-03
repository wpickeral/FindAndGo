using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FindAndGo.Models;
using FindAndGo.Services;

namespace FindAndGo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private IKrogerService _krogerService;

    public HomeController(ILogger<HomeController> logger, IKrogerService krogerService)
    {
        _logger = logger;
        _krogerService = krogerService;
    }

    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Request.Cookies["find-and-go.token"];

        if (token == null) // Try to get a new token
        {
            try
            {
                var newTokenRequest = await _krogerService.GetAccessToken();
                if (newTokenRequest != null)
                {
                    ControllerHelpers.AddTokenAsCookieToResponse(newTokenRequest, HttpContext);
                    token = newTokenRequest["access_token"].ToString();
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                return View("PageNotFound");
            }
        }

        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}