using Azunt.Web.Models.Markdown;
using Azunt.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

[Route("resources")]
public sealed class ResourcesController(MarkdownContentService markdownContent) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();

    // Each action maps an existing Markdown document to a public MVC URL.
    // The source document remains available through DocFX under /docs/.
    [HttpGet("authentication")]
    public Task<IActionResult> Authentication() => RenderMarkdownPageAsync(
        "guides/authentication.md",
        "/resources/authentication",
        "/docs/guides/authentication");

    [HttpGet("routing")]
    public Task<IActionResult> Routing() => RenderMarkdownPageAsync(
        "code/routing.md",
        "/resources/routing",
        "/docs/code/routing");

    [HttpGet("site-structure")]
    public Task<IActionResult> SiteStructure() => RenderMarkdownPageAsync(
        "getting-started/site-structure.md",
        "/resources/site-structure",
        "/docs/getting-started/site-structure");

    private async Task<IActionResult> RenderMarkdownPageAsync(
        string markdownPath,
        string currentUrl,
        string canonicalUrl)
    {
        var model = await markdownContent.GetFilePageAsync(new MarkdownFilePageRequest
        {
            ContentRoot = "MarkdownDocs",
            MarkdownPath = markdownPath,
            CurrentUrl = currentUrl,
            RootTitle = "Resources",
            RootUrl = "/resources",
            NavigationBaseUrl = "/resources",
            TocMode = MarkdownTocMode.Explicit,
            TocPath = "mappings/resources.yml",
            NavigationTitle = "Selected documentation",
            CanonicalUrl = canonicalUrl,
            BuildSourceBreadcrumbs = false
        });

        return model is null
            ? NotFound()
            : View("~/Views/Shared/MarkdownPage.cshtml", model);
    }
}
