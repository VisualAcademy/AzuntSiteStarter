using Azunt.Web.Models.Docs;

namespace Azunt.Web.Models.Markdown;

public sealed class MarkdownPageViewModel
{
    public required string Title { get; init; }
    public required string Html { get; init; }
    public required string Slug { get; init; }
    public required string SourcePath { get; init; }
    public required string RootTitle { get; init; }
    public string? RootUrl { get; init; }
    public required string NavigationTitle { get; init; }
    public required IReadOnlyList<MarkdownNavigationItemViewModel> Navigation { get; init; }
    public required IReadOnlyList<DocsHeadingViewModel> OnThisPage { get; init; }
    public required IReadOnlyList<MarkdownBreadcrumbViewModel> Breadcrumbs { get; init; }
    public string? CanonicalUrl { get; init; }
}
