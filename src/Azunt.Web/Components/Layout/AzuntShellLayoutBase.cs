using Azunt.Web.Models.Shells;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace Azunt.Web.Components.Layout;

public abstract class AzuntShellLayoutBase : LayoutComponentBase
{
    [CascadingParameter]
    protected Microsoft.AspNetCore.Components.RouteData? CurrentRouteData { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    protected AzuntShellMode ShellMode { get; private set; } = AzuntShellMode.Public;
    protected string ShellModeName => ShellMode.ToString().ToLowerInvariant();

    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationStateTask;
        ShellMode = AzuntShellComponentData.ResolveMode(CurrentRouteData, authState.User);
    }
}
