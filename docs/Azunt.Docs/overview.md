# Documentation overview

Markdown files start directly in the `Azunt.Docs` project root.

```text
Azunt.Docs/
├─ index.md
├─ overview.md
├─ toc.yml
├─ navigation/
│  └─ toc.yml
├─ getting-started/
├─ guides/
├─ code/
├─ protected/
├─ templates/
└─ docfx.json
```

There is no intermediate `articles`, `guide`, or nested `docs` content directory.

## Source path and URL

```text
index.md                              /docs/
overview.md                           /docs/overview
getting-started/site-structure.md     /docs/getting-started/site-structure
guides/authentication.md              /docs/guides/authentication
code/razor.md                         /docs/code/razor
```

DocFX produces HTML files. ASP.NET Core maps extensionless `/docs/...` URLs to those files.

## TreeView

The two TOC files have separate jobs:

```text
toc.yml                 top navigation
navigation/toc.yml      left TreeView
```

`navigation/toc.yml` references Markdown files with relative paths such as `../overview.md` and `../guides/authentication.md`. DocFX uses this local TOC for the left navigation.

To add a public page:

1. Add the Markdown file at the desired path.
2. Add it to `navigation/toc.yml`.
3. Rebuild the solution.

## Protected documentation

Files under `protected/` are excluded from the public DocFX build. ASP.NET Core copies those files to application output and renders them through an `[Authorize]` MVC route.

Protected pages use the same three-part docs layout: left navigation, article content, and `On this page` links.
