# AzuntSiteStarter

ASP.NET Core 10 reference solution with four site areas under one host:

- `/` — public pages
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

The web project runs DocFX 2.78.5 through `dotnet tool exec`. There is no `dotnet tool restore` target inside the Visual Studio build.

## Test account

- Email: `demo@azunt.local`
- Password: `Azunt123!`

Authentication uses ASP.NET Core Identity with EF Core InMemory. The data is cleared when the application process restarts. Use SQL Server or Azure SQL for persistent identity storage.

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
│     ├─ navigation/
│     │  └─ toc.yml
│     ├─ getting-started/
│     ├─ guides/
│     ├─ code/
│     ├─ protected/
│     ├─ templates/azunt/
│     └─ docfx.json
├─ Directory.Build.props
├─ Directory.Packages.props
└─ global.json
```

Markdown starts directly at the `Azunt.Docs` root. There is no nested `articles`, `guide`, or `docs` content folder.

## Public documentation

Source paths map directly to public URLs:

```text
index.md                              /docs/
overview.md                           /docs/overview
getting-started/site-structure.md     /docs/getting-started/site-structure
guides/authentication.md              /docs/guides/authentication
code/razor.md                         /docs/code/razor
```

DocFX writes HTML files internally. ASP.NET Core exposes extensionless `/docs/...` URLs.

### Left TreeView

DocFX uses two TOC files:

```text
toc.yml                 top navigation
navigation/toc.yml      left TreeView
```

Add a Markdown file at the desired URL path, then add it to `navigation/toc.yml`.

## Protected documentation

Markdown under `docs/Azunt.Docs/protected` is excluded from the public DocFX build. The web project copies it to the application output and serves it through `DocsController` after authorization.

```text
docs/Azunt.Docs/protected/*.md
        ↓
ProtectedDocs/*.md
        ↓
DocsController + [Authorize]
        ↓
Markdig
        ↓
/docs/private/*
```

Public and protected docs use the same visual structure: left navigation, centered article content, and an `On this page` rail when headings are available. Public pages use the DocFX modern template; protected pages use `_DocsLayout.cshtml`.

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

`Azunt.Web.csproj` registers the DocFX source tree with Visual Studio's fast up-to-date check. Documentation changes therefore trigger the DocFX build even though the files are outside `src/Azunt.Web`.

The full workflow and the `Microsoft.WebTools.ProjectSystem.WebServer.SelfHostWebServer` recovery steps are documented at:

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

The deployed application does not require DocFX at runtime.

To skip the DocFX build temporarily:

```powershell
dotnet build .\src\Azunt.Web\Azunt.Web.csproj -p:BuildDocsOnBuild=false
```

## Razor and CSHTML highlighting

The DocFX template extension is under:

```text
docs/Azunt.Docs/templates/azunt
```

`public/main.js` registers the Razor grammar from `razor.js` for both `razor` and `cshtml` fenced code blocks.

Test page:

```text
/docs/code/razor
```

## Dashboard navigation

The dashboard sidebar supports nested menu levels.

```text
Overview
Resources
  App services
  Databases
  Storage
Management
  Identity
    Users
    Roles
  Tenants
Monitoring
  Activity log
  Service health
Documentation
Protected docs
Account
```

Navigation data is defined in `DashboardNavigationViewComponent.cs`. `_DashboardNavNode.cshtml` renders the tree recursively.

The two sidebar controls have separate behavior:

- Top hamburger — slides the entire sidebar out or back in.
- Bottom arrow — switches between the full sidebar and the compact icon rail.

In compact mode, parent icons open a dark flyout tree. Flyout list markers are removed; hierarchy is shown with icons, indentation, and chevrons.

## Main extension points

The current solution keeps one executable web project. If the codebase grows, common extraction points are:

- `Azunt.UI` — reusable layouts and components
- `Azunt.Application` — application services and use cases
- `Azunt.Domain` — domain models and rules
- `Azunt.Infrastructure` — SQL Server, Azure, and external integrations
- authorization policies for product-, tenant-, role-, or license-specific documentation
