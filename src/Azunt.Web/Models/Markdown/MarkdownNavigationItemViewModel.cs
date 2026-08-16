namespace Azunt.Web.Models.Markdown;

public sealed class MarkdownNavigationItemViewModel
{
    public required string Name { get; init; }
    public string? Href { get; init; }
    public bool IsActive { get; init; }
    public bool IsInActivePath { get; init; }
    public IReadOnlyList<MarkdownNavigationItemViewModel> Items { get; init; } = [];
}
