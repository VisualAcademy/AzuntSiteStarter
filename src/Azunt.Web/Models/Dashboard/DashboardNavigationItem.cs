namespace Azunt.Web.Models.Dashboard;

public sealed record DashboardNavigationItem(
    string Key,
    string Text,
    string Icon,
    string Href,
    IReadOnlyList<DashboardNavigationItem>? Children = null,
    bool ExpandedByDefault = false,
    string? Group = null);
