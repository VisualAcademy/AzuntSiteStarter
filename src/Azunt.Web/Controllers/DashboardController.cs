using Azunt.Web.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.Controllers;

[Authorize]
[Route("dashboard")]
public sealed class DashboardController : Controller
{
    private static readonly IReadOnlyDictionary<string, DashboardPageViewModel> Pages =
        new Dictionary<string, DashboardPageViewModel>(StringComparer.OrdinalIgnoreCase)
        {
            ["/dashboard/resources"] = Page(
                "Resources",
                "Resource categories available from the dashboard.",
                "Dashboard / Resources",
                "App services", "Databases", "Storage"),
            ["/dashboard/resources/app-services"] = Page(
                "App services",
                "Application service inventory and configuration.",
                "Dashboard / Resources / App services",
                "Web applications", "Deployment slots", "Runtime configuration"),
            ["/dashboard/resources/databases"] = Page(
                "Databases",
                "Database inventory, connection status, and backups.",
                "Dashboard / Resources / Databases",
                "SQL resources", "Connection health", "Backup status"),
            ["/dashboard/resources/storage"] = Page(
                "Storage",
                "Blob, file, queue, and application storage.",
                "Dashboard / Resources / Storage",
                "Blob containers", "Files", "Usage"),
            ["/dashboard/management"] = Page(
                "Management",
                "Identity, tenants, and access configuration.",
                "Dashboard / Management",
                "Identity", "Tenants", "Policies"),
            ["/dashboard/management/identity"] = Page(
                "Identity",
                "Users, roles, and access policies.",
                "Dashboard / Management / Identity",
                "Users", "Roles", "Access policies"),
            ["/dashboard/management/identity/users"] = Page(
                "Users",
                "User accounts, invitations, and access reviews.",
                "Dashboard / Management / Identity / Users",
                "User list", "Invitations", "Access review"),
            ["/dashboard/management/identity/roles"] = Page(
                "Roles",
                "Role definitions, assignments, and permissions.",
                "Dashboard / Management / Identity / Roles",
                "Role list", "Assignments", "Permissions"),
            ["/dashboard/management/tenants"] = Page(
                "Tenants",
                "Tenant records, subscriptions, and configuration.",
                "Dashboard / Management / Tenants",
                "Tenant list", "Subscriptions", "Configuration"),
            ["/dashboard/monitoring"] = Page(
                "Monitoring",
                "Operational history, health, and alerts.",
                "Dashboard / Monitoring",
                "Activity log", "Service health", "Alerts"),
            ["/dashboard/monitoring/activity"] = Page(
                "Activity log",
                "Recent operations and audit history.",
                "Dashboard / Monitoring / Activity log",
                "Recent operations", "Audit trail", "Filters"),
            ["/dashboard/monitoring/health"] = Page(
                "Service health",
                "Availability, incidents, and maintenance.",
                "Dashboard / Monitoring / Service health",
                "Availability", "Incidents", "Maintenance")
        };

    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("resources")]
    [HttpGet("resources/app-services")]
    [HttpGet("resources/databases")]
    [HttpGet("resources/storage")]
    [HttpGet("management")]
    [HttpGet("management/identity")]
    [HttpGet("management/identity/users")]
    [HttpGet("management/identity/roles")]
    [HttpGet("management/tenants")]
    [HttpGet("monitoring")]
    [HttpGet("monitoring/activity")]
    [HttpGet("monitoring/health")]
    public IActionResult Section()
    {
        var path = Request.Path.Value?.TrimEnd('/') ?? string.Empty;
        if (!Pages.TryGetValue(path, out var page))
        {
            return NotFound();
        }

        return View(page);
    }

    private static DashboardPageViewModel Page(
        string title,
        string description,
        string breadcrumb,
        params string[] highlights) => new()
        {
            Title = title,
            Description = description,
            Breadcrumb = breadcrumb,
            Highlights = highlights
        };
}
