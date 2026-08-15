# Protected documentation

This Markdown file is excluded from the public DocFX build. Requests to `/docs/private/` pass through ASP.NET Core authentication and authorization before MVC renders the file with Markdig.

## Authorization

The current controller uses `[Authorize]`. It can be replaced with a policy:

```csharp
[Authorize(Policy = "VendorLicensingCustomer")]
[Route("docs/private/vendor-licensing")]
public sealed class VendorLicensingDocsController : Controller
{
}
```

The same approach can be used for role, tenant, subscription, product license, or organization checks.

## Razor example

```razor
@page "/account/settings"
@attribute [Authorize]

<h1>Settings</h1>

@if (User.Identity?.IsAuthenticated == true)
{
    <p>Welcome, @User.Identity.Name</p>
}
```

## Test account

- `demo@azunt.local`
- `Azunt123!`

EF Core InMemory data is cleared when the application restarts.
