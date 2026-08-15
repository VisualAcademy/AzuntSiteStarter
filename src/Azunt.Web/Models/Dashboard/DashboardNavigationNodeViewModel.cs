namespace Azunt.Web.Models.Dashboard;

public sealed class DashboardNavigationNodeViewModel
{
    public required string Key { get; init; }
    public required string Text { get; init; }
    public required string Icon { get; init; }
    public required string Href { get; init; }
    public required int Depth { get; init; }
    public required bool IsCurrent { get; init; }
    public required bool ContainsCurrent { get; init; }
    public required bool IsExpanded { get; init; }
    public string? Group { get; init; }
    public required IReadOnlyList<DashboardNavigationNodeViewModel> Children { get; init; }
}
