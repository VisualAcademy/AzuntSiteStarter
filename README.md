# AzuntSiteStarter

ASP.NET Core 10 reference solution with public pages, Markdown content, DocFX documentation, dashboard, and account areas under one host.

- `/` — public pages
- `/courses/` — folder-based Markdown rendered inside the public layout
- `/resources/` — selected DocFX Markdown files mapped explicitly to MVC pages
- `/docs/` — DocFX 2.78.5 public documentation
- `/docs/private/` — authenticated Markdown documentation rendered by MVC
- `/dashboard/` — authenticated dashboard
- `/account/` — sign-in, registration, and profile pages

## Requirements

- .NET 10 SDK
- Visual Studio with .NET 10 and `.slnx` support, or another .NET 10 compatible editor
- Internet access on the first restore/build for NuGet packages and DocFX 2.78.5

## Run

Open `Azunt.Site.slnx`, set `Azunt.Web` as the startup project, and run it. On Windows you can also use `Run.cmd`.

```powershell
dotnet restore
dotnet run --project .\src\Azunt.Web\Azunt.Web.csproj --launch-profile https
```

Convenience commands:

- `Build.cmd` — restore and build
- `Run.cmd` — run the HTTPS launch profile
- `Publish.cmd` — publish Release output to `artifacts/publish`

## Test account

- Email: `demo@azunt.local`
- Password: `Azunt123!`

Authentication uses ASP.NET Core Identity with EF Core InMemory. The data is cleared when the application process restarts.

## Solution layout

```text
AzuntSiteStarter/
├─ Azunt.Site.slnx
├─ src/
│  └─ Azunt.Web/
├─ docs/
│  └─ Azunt.Docs/
│     ├─ index.md
│     ├─ overview.md
│     ├─ toc.yml
│     ├─ navigation/toc.yml
│     ├─ getting-started/
│     ├─ guides/
│     ├─ code/
│     ├─ protected/
│     ├─ courses/
│     ├─ mappings/
│     │  └─ resources.yml
│     ├─ templates/azunt/
│     └─ docfx.json
├─ Directory.Build.props
├─ Directory.Packages.props
└─ global.json
```

Markdown starts directly at the `Azunt.Docs` root.

## MarkdownContentService

MVC Markdown rendering is handled by one service:

```text
MarkdownContentService
├─ Courses
├─ Protected docs
└─ Explicit MVC mappings
```

The service handles Markdown rendering, heading extraction, breadcrumbs, YAML navigation, nearest-folder TOC lookup, explicit TOC selection, and safe path resolution.

### Folder mapping

`/courses/**` maps a complete Markdown folder tree to one MVC URL space.

```text
courses/aspnet-core/fundamentals/routing.md
→ /courses/aspnet-core/fundamentals/routing
```

The nearest `toc.yml` is selected automatically.

### Explicit MVC mapping

A controller can map one existing public documentation file to another MVC URL.

```csharp
[HttpGet("authentication")]
public Task<IActionResult> Authentication() => RenderMarkdownPageAsync(
    "guides/authentication.md",
    "/resources/authentication",
    "/docs/guides/authentication");
```

This produces both URLs from one Markdown source:

```text
/docs/guides/authentication      DocFX layout
/resources/authentication       Public MVC layout
```

The MVC page uses `mappings/resources.yml` for its local left navigation. The canonical URL points to the DocFX page in the sample.

Additional examples:

```text
/resources/routing
/resources/site-structure
```

The implementation is documented at:

```text
/docs/guides/mvc-markdown-mapping
```

## Public documentation

Source paths map directly to public DocFX URLs:

```text
index.md                              /docs/
overview.md                           /docs/overview
getting-started/site-structure.md     /docs/getting-started/site-structure
guides/authentication.md              /docs/guides/authentication
code/razor.md                         /docs/code/razor
```

DocFX uses:

```text
toc.yml                 top navigation
navigation/toc.yml      left TreeView
```

## Courses in the public layout

Markdown under `docs/Azunt.Docs/courses` is rendered inside `_PublicLayout.cshtml`. The public header and footer are retained, while the documentation body stays inside the same `shell-width` container used by landing pages.

```text
Public header
────────────────────────────────────────
Local TOC | Article | On this page
────────────────────────────────────────
Public footer
```

A literal controller route can coexist with the Markdown catch-all. `/courses/mvc-page` is included as a route-priority example.

## Protected documentation

Markdown under `docs/Azunt.Docs/protected` is excluded from DocFX and copied to `ProtectedDocs` in the web output. `DocsController` runs authorization before calling `MarkdownContentService`.

```text
protected/*.md
   ↓
[Authorize]
   ↓
MarkdownContentService
   ↓
/docs/private/*
```

## Runtime Markdown copies

The web project keeps Markdown source outside `wwwroot` for MVC rendering:

```text
CourseDocs/       courses/**
ProtectedDocs/    protected/**
MarkdownDocs/     public DocFX Markdown used by explicit mappings
```

These folders are generated in the build/publish output from the source under `docs/Azunt.Docs`.

## Visual Studio documentation workflow

Start the web application once with `Ctrl + F5`. For Markdown, TOC, DocFX config, or DocFX template changes, keep the application running:

```text
Edit docs
   ↓
Ctrl + Shift + B
   ↓
DocFX rebuilds
   ↓
Refresh browser
```

The detailed workflow is at:

```text
/docs/getting-started/development-workflow
```

## DocFX build and publish

Development output:

```text
src/Azunt.Web/obj/DocFxSite
```

Publish output:

```text
wwwroot/docs/**
```

To skip DocFX temporarily:

```powershell
dotnet build .\src\Azunt.Web\Azunt.Web.csproj -p:BuildDocsOnBuild=false
```

## Dashboard

The dashboard supports nested navigation, a sliding sidebar, compact icon rail, flyout tree menus, and the waffle app launcher.

- Top hamburger — hides or restores the whole sidebar
- Bottom arrow — switches between full sidebar and icon rail
- Compact parent icon — opens the submenu flyout

## Main extension points

- `MarkdownContentService` — shared Markdown renderer for folder, protected, and explicit MVC mappings
- `DashboardNavigationViewComponent` — dashboard navigation data
- `Azunt.UI` — future reusable layouts/components
- `Azunt.Application` — future application services/use cases
- `Azunt.Domain` — future domain models/rules
- `Azunt.Infrastructure` — future SQL Server, Azure, and external integrations
