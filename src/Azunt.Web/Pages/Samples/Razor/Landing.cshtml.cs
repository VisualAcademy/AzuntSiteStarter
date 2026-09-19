using Azunt.Web.Models.Shells;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Azunt.Web.Pages.Samples.Razor;

public sealed class LandingModel : PageModel
{
    public void OnGet()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Landing, AzuntShellMode.Public);
    }
}
