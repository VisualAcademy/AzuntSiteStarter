using Azunt.Web.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.ViewComponents;

public sealed class DashboardNavigationViewComponent : ViewComponent
{
    private static readonly IReadOnlyList<DashboardNavigationItem> Navigation =
    [
        new("overview", "Overview", "home", "/dashboard", Group: "AZUNT PORTAL"),
        new(
            "resources",
            "Resources",
            "resources",
            "/dashboard/resources",
            [
                new("app-services", "App services", "app-service", "/dashboard/resources/app-services"),
                new("databases", "Databases", "database", "/dashboard/resources/databases"),
                new("storage", "Storage", "storage", "/dashboard/resources/storage")
            ],
            Group: "AZUNT PORTAL"),
        new(
            "management",
            "Management",
            "settings",
            "/dashboard/management",
            [
                new(
                    "identity",
                    "Identity",
                    "identity",
                    "/dashboard/management/identity",
                    [
                        new("users", "Users", "users", "/dashboard/management/identity/users"),
                        new("roles", "Roles", "roles", "/dashboard/management/identity/roles")
                    ]),
                new("tenants", "Tenants", "tenants", "/dashboard/management/tenants")
            ],
            Group: "MANAGEMENT"),
        new(
            "monitoring",
            "Monitoring",
            "monitoring",
            "/dashboard/monitoring",
            [
                new("activity", "Activity log", "activity", "/dashboard/monitoring/activity"),
                new("health", "Service health", "health", "/dashboard/monitoring/health")
            ],
            Group: "MANAGEMENT"),
        new("documentation", "Documentation", "docs", "/docs/", Group: "LINKS"),
        new("protected-docs", "Protected docs", "shield", "/docs/private/", Group: "LINKS"),
        new("account", "Account", "account", "/account/profile", Group: "LINKS")
    ];

    public IViewComponentResult Invoke()
    {
        var currentPath = Normalize(HttpContext.Request.Path.Value);
        var nodes = Navigation.Select(item => BuildNode(item, currentPath, 0)).ToArray();

        return View(new DashboardNavigationViewModel
        {
            Items = nodes
        });
    }

    private static DashboardNavigationNodeViewModel BuildNode(
        DashboardNavigationItem item,
        string currentPath,
        int depth)
    {
        var children = (item.Children ?? [])
            .Select(child => BuildNode(child, currentPath, depth + 1))
            .ToArray();

        var href = Normalize(item.Href);
        var isCurrent = string.Equals(currentPath, href, StringComparison.OrdinalIgnoreCase);
        var containsCurrent = isCurrent || children.Any(child => child.ContainsCurrent);

        return new DashboardNavigationNodeViewModel
        {
            Key = item.Key,
            Text = item.Text,
            Icon = item.Icon,
            Href = item.Href,
            Depth = depth,
            IsCurrent = isCurrent,
            ContainsCurrent = containsCurrent,
            IsExpanded = item.ExpandedByDefault || containsCurrent,
            Group = item.Group,
            Children = children
        };
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "/";
        }

        var normalized = value.Trim();
        if (normalized.Length > 1)
        {
            normalized = normalized.TrimEnd('/');
        }

        return normalized.ToLowerInvariant();
    }
}
