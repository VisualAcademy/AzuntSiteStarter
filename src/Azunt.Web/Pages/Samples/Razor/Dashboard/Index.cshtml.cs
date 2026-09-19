using Azunt.Web.Models.Shells;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Azunt.Web.Pages.Samples.Razor.Dashboard;

[Authorize]
public sealed class IndexModel : PageModel
{
    public void OnGet()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Dashboard, AzuntShellMode.Authenticated);
    }
}
