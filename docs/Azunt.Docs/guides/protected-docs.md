# Protected documentation

Public DocFX output is static. Markdown that requires authentication stays outside `wwwroot` and is rendered through MVC.

```text
Markdown outside wwwroot
        ↓
ASP.NET Core routing
        ↓
Authentication / Authorization
        ↓
Markdig
        ↓
DocsShell
```

The protected route is:

```text
/docs/private/
```

`DocsController` uses `[Authorize]`. Anonymous requests are redirected to `/account/login` and return to the requested page after sign-in.

## Layout

Public and protected documentation use the same page structure:

```text
Left navigation | Article | On this page
```

DocFX renders the public layout. `_DocsLayout.cshtml` renders the protected layout. The protected layout extracts H2/H3 headings from the rendered Markdown and builds the right-side `On this page` navigation.

## Authorization policies

The controller can use policies instead of plain `[Authorize]`:

```csharp
[Authorize(Policy = "VendorLicensingCustomer")]
```

Policies can check tenant, product, role, license, or other application claims.
