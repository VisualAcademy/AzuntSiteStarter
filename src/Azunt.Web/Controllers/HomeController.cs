using System.Diagnostics;
using Azunt.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

public sealed class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View();

    [HttpGet("/products")]
    public IActionResult Products() => View();

    [HttpGet("/pricing")]
    public IActionResult Pricing() => View();

    [HttpGet("/home/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
