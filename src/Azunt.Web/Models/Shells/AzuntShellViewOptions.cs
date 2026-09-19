using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Azunt.Web.Models.Shells;

/// <summary>
/// Lightweight metadata describing the active Azunt UI shell.
/// The shell controls presentation; authorization remains the responsibility
/// of ASP.NET Core authorization attributes/policies.
/// </summary>
public sealed record AzuntShellViewOptions(AzuntShellKind Kind, AzuntShellMode Mode)
{
    public string KindName => Kind.ToString().ToLowerInvariant();
    public string ModeName => Mode.ToString().ToLowerInvariant();
}

public static class AzuntShellViewData
{
    private const string OptionsKey = "Azunt.Shell.Options";

    public static void Set(
        ViewDataDictionary viewData,
        AzuntShellKind kind,
        AzuntShellMode mode)
    {
        ArgumentNullException.ThrowIfNull(viewData);
        viewData[OptionsKey] = new AzuntShellViewOptions(kind, mode);
    }

    public static AzuntShellViewOptions Resolve(
        ViewDataDictionary viewData,
        AzuntShellKind expectedKind,
        ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(viewData);
        ArgumentNullException.ThrowIfNull(user);

        if (viewData[OptionsKey] is AzuntShellViewOptions configured && configured.Kind == expectedKind)
        {
            return configured;
        }

        var mode = user.Identity?.IsAuthenticated == true
            ? AzuntShellMode.Authenticated
            : AzuntShellMode.Public;

        return new AzuntShellViewOptions(expectedKind, mode);
    }
}
