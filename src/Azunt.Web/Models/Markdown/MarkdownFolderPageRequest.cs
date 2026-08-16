namespace Azunt.Web.Models.Markdown;

public sealed class MarkdownFolderPageRequest
{
    public required string ContentRoot { get; init; }
    public required string BaseUrl { get; init; }
    public required string RootTitle { get; init; }
    public string? Slug { get; init; }
    public string DefaultSlug { get; init; } = "index";
    public MarkdownTocMode TocMode { get; init; } = MarkdownTocMode.Nearest;
    public string? TocPath { get; init; }
    public string? NavigationTitle { get; init; }
    public string? CanonicalUrl { get; init; }
    public bool BuildSourceBreadcrumbs { get; init; } = true;
}
