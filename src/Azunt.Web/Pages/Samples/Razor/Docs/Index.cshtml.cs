using Azunt.Web.Models.Shells;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Azunt.Web.Pages.Samples.Razor.Docs;

public sealed class IndexModel : PageModel
{
    public void OnGet()
    {
        AzuntShellViewData.Set(ViewData, AzuntShellKind.Docs, AzuntShellMode.Public);
    }
}
