using Azunt.Web.Models.Shells;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Azunt.Web.Pages.Samples.Razor.Docs;

[Authorize]
public sealed class PrivateModel : PageModel
{
    public void OnGet()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Docs, AzuntShellMode.Private);
    }
}
