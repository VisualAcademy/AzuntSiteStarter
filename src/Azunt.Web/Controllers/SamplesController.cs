using Azunt.Web.Models.Shells;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

/// <summary>
/// MVC shell smoke-test pages. These pages intentionally contain very little
/// application logic so visual regressions in each shell are easy to spot.
/// </summary>
[Route("samples/mvc")]
public sealed class SamplesController : Controller
{
    [AllowAnonymous]
    [HttpGet("landing")]
    public IActionResult Landing()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Landing, AzuntShellMode.Public);
        return View();
    }

    [AllowAnonymous]
    [HttpGet("docs")]
    public IActionResult Docs()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Docs, AzuntShellMode.Public);
        return View();
    }

    [Authorize]
    [HttpGet("docs/authenticated")]
    public IActionResult AuthenticatedDocs()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Docs, AzuntShellMode.Authenticated);
        return View("Docs");
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("docs/admin")]
    public IActionResult AdminDocs()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Docs, AzuntShellMode.Admin);
        return View("Docs");
    }

    [Authorize]
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Dashboard, AzuntShellMode.Authenticated);
        return View();
    }
}
