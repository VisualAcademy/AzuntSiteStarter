using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;

namespace Azunt.Web.Models.Shells;

/// <summary>
/// Optional route-component metadata that selects a visual shell mode without
/// coupling the visual mode to ASP.NET Core authorization rules.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class AzuntShellModeAttribute(AzuntShellMode mode) : Attribute
{
    public AzuntShellMode Mode { get; } = mode;
}

public static class AzuntShellComponentData
{
    public static AzuntShellMode ResolveMode(Microsoft.AspNetCore.Components.RouteData? routeData, ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var configuredMode = routeData?.PageType
            .GetCustomAttribute<AzuntShellModeAttribute>(inherit: true)
            ?.Mode;

        if (configuredMode.HasValue)
        {
            return configuredMode.Value;
        }

        return user.Identity?.IsAuthenticated == true
            ? AzuntShellMode.Authenticated
            : AzuntShellMode.Public;
    }
}
