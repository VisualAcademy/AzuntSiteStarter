# Site structure

One ASP.NET Core host serves four layouts.

```text
azunt.com/
├─ /                  PublicShell
├─ /docs/             DocsShell (DocFX)
├─ /docs/private/     DocsShell (MVC + authorization)
├─ /dashboard/        DashboardShell
└─ /account/          AccountShell
```

## Solution layout

```text
Azunt.Site/
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
│     ├─ templates/
│     └─ docfx.json
├─ Directory.Build.props
├─ Directory.Packages.props
└─ global.json
```

DocFX source stays outside `wwwroot`. Generated documentation is a build artifact.

## Documentation navigation

Public Markdown remains directly under `Azunt.Docs/` and its subject folders. The root `toc.yml` handles top navigation. `navigation/toc.yml` handles the native DocFX left TreeView.

When documentation changes, update the Markdown and `navigation/toc.yml`, rebuild, and refresh the browser.

## Project boundaries

If the solution grows, layouts and shared UI can move to a Razor Class Library, while application, domain, and infrastructure code can move into separate projects.
