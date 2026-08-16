using Azunt.Web.Models.Markdown;
using Azunt.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

[Authorize]
[Route("docs/private")]
public sealed class DocsController(MarkdownContentService markdownContent) : Controller
{
    [HttpGet("")]
    [HttpGet("{*slug}")]
    public async Task<IActionResult> Private(string? slug)
    {
        var model = await markdownContent.GetFolderPageAsync(new MarkdownFolderPageRequest
        {
            ContentRoot = "ProtectedDocs",
            BaseUrl = "/docs/private",
            RootTitle = "Protected",
            Slug = slug,
            DefaultSlug = "overview",
            TocMode = MarkdownTocMode.None,
            BuildSourceBreadcrumbs = false
        });

        return model is null ? NotFound() : View("Private", model);
    }
}
