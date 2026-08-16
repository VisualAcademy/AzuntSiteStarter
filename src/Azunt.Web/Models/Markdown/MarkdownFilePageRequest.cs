namespace Azunt.Web.Models.Markdown;

public sealed class MarkdownFilePageRequest
{
    public required string ContentRoot { get; init; }
    public required string MarkdownPath { get; init; }
    public required string CurrentUrl { get; init; }
    public required string RootTitle { get; init; }
    public string? RootUrl { get; init; }
    public string NavigationBaseUrl { get; init; } = "/";
    public MarkdownTocMode TocMode { get; init; } = MarkdownTocMode.None;
    public string? TocPath { get; init; }
    public string? NavigationTitle { get; init; }
    public string? CanonicalUrl { get; init; }
    public bool BuildSourceBreadcrumbs { get; init; }
}
