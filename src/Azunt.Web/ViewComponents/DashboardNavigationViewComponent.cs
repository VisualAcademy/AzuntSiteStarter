using Azunt.Web.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Azunt.Web.ViewComponents;

public sealed class DashboardNavigationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = DashboardNavigationCatalog.Build(HttpContext.Request.Path.Value);
        return View(model);
    }
}
