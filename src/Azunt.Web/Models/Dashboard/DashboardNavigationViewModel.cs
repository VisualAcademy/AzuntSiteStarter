namespace Azunt.Web.Models.Dashboard;

public sealed class DashboardNavigationViewModel
{
    public required IReadOnlyList<DashboardNavigationNodeViewModel> Items { get; init; }
}
