namespace Azunt.Web.Models.Dashboard;

public sealed class DashboardPageViewModel
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Breadcrumb { get; init; }
    public required IReadOnlyList<string> Highlights { get; init; }
}
