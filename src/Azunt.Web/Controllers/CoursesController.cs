using Azunt.Web.Models.Markdown;
using Azunt.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

[Route("courses")]
public sealed class CoursesController(MarkdownContentService markdownContent) : Controller
{
    [HttpGet("")]
    public Task<IActionResult> Index() => RenderMarkdownPageAsync(null);

    [HttpGet("mvc-page")]
    public IActionResult MvcPage() => View();

    [HttpGet("{*slug}", Order = 1000)]
    public Task<IActionResult> Page(string? slug) => RenderMarkdownPageAsync(slug);

    private async Task<IActionResult> RenderMarkdownPageAsync(string? slug)
    {
        var model = await markdownContent.GetFolderPageAsync(new MarkdownFolderPageRequest
        {
            ContentRoot = "CourseDocs",
            BaseUrl = "/courses",
            RootTitle = "Courses",
            Slug = slug,
            TocMode = MarkdownTocMode.Nearest,
            BuildSourceBreadcrumbs = true
        });

        return model is null
            ? NotFound()
            : View("~/Views/Shared/MarkdownPage.cshtml", model);
    }
}
