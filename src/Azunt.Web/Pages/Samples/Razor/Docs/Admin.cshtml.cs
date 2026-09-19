using Azunt.Web.Models.Shells;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Azunt.Web.Pages.Samples.Razor.Docs;

[Authorize(Roles = "Administrator")]
public sealed class AdminModel : PageModel
{
    public void OnGet()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Docs, AzuntShellMode.Admin);
    }
}
