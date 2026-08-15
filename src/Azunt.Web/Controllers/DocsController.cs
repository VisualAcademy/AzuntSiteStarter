using System.Net;
using System.Text.RegularExpressions;
using Azunt.Web.Models.Docs;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

[Authorize]
[Route("docs/private")]
public sealed partial class DocsController : Controller
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    [HttpGet("")]
    [HttpGet("{*slug}")]
    public async Task<IActionResult> Private(string? slug)
    {
        slug = string.IsNullOrWhiteSpace(slug) ? "overview" : slug.Trim('/');

        if (!SafeSlugRegex().IsMatch(slug))
        {
            return BadRequest();
        }

        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "ProtectedDocs"));
        var filePath = Path.GetFullPath(Path.Combine(root, slug.Replace('/', Path.DirectorySeparatorChar) + ".md"));

        if (!filePath.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var markdown = await System.IO.File.ReadAllTextAsync(filePath);
        var title = ExtractTitle(markdown) ?? slug.Split('/').Last().Replace('-', ' ');
        var html = Markdown.ToHtml(markdown, Pipeline);

        return View("Private", new DocsPageViewModel
        {
            Title = title,
            Html = html,
            Slug = slug,
            OnThisPage = ExtractHeadings(html)
        });
    }

    private static string? ExtractTitle(string markdown)
    {
        using var reader = new StringReader(markdown);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                return line[2..].Trim();
            }
        }

        return null;
    }

    private static IReadOnlyList<DocsHeadingViewModel> ExtractHeadings(string html)
    {
        var headings = new List<DocsHeadingViewModel>();

        foreach (Match match in HeadingRegex().Matches(html))
        {
            var id = match.Groups["id"].Value;
            if (string.IsNullOrWhiteSpace(id))
            {
                continue;
            }

            var text = HtmlTagRegex().Replace(match.Groups["text"].Value, string.Empty);
            text = WebUtility.HtmlDecode(text).Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            headings.Add(new DocsHeadingViewModel
            {
                Id = id,
                Text = text,
                Level = int.Parse(match.Groups["level"].Value)
            });
        }

        return headings;
    }

    [GeneratedRegex("^[a-zA-Z0-9/_-]+$")]
    private static partial Regex SafeSlugRegex();

    [GeneratedRegex("<h(?<level>[23])\\s+id=\"(?<id>[^\"]+)\"[^>]*>(?<text>.*?)</h\\k<level>>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex HeadingRegex();

    [GeneratedRegex("<[^>]+>", RegexOptions.Singleline)]
    private static partial Regex HtmlTagRegex();
}
