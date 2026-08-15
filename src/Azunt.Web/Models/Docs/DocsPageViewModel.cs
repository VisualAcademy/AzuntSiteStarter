namespace Azunt.Web.Models.Docs;

public sealed class DocsPageViewModel
{
    public required string Title { get; init; }
    public required string Html { get; init; }
    public required string Slug { get; init; }
    public required IReadOnlyList<DocsHeadingViewModel> OnThisPage { get; init; }
}
