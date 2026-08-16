# Map a Markdown file to an MVC page

A Markdown file can remain part of the DocFX documentation and also be rendered by an explicit MVC route.

For example, this source file:

```text
guides/authentication.md
```

is available through DocFX at:

```text
/docs/guides/authentication
```

The same file is also mapped by `ResourcesController` to:

```text
/resources/authentication
```

The MVC route uses the public header and footer and renders the article inside the container-based three-column content layout.

## Controller mapping

`ResourcesController` maps a source file explicitly:

```csharp
[HttpGet("authentication")]
public Task<IActionResult> Authentication() => RenderMarkdownPageAsync(
    "guides/authentication.md",
    "/resources/authentication",
    "/docs/guides/authentication");
```

The helper calls `MarkdownContentService` with the runtime documentation copy named `MarkdownDocs`.

```csharp
var model = await markdownContent.GetFilePageAsync(new MarkdownFilePageRequest
{
    ContentRoot = "MarkdownDocs",
    MarkdownPath = markdownPath,
    CurrentUrl = currentUrl,
    RootTitle = "Resources",
    RootUrl = "/resources",
    TocMode = MarkdownTocMode.Explicit,
    TocPath = "mappings/resources.yml",
    CanonicalUrl = canonicalUrl
});
```

## Local navigation

An MVC mapping may use no TOC, the nearest `toc.yml`, or an explicitly selected YAML file.

The sample resources pages use:

```text
mappings/resources.yml
```

This file contains only the MVC pages that should appear in the left navigation.

```yaml
- name: Authentication
  href: /resources/authentication

- name: Routing
  href: /resources/routing

- name: Site structure
  href: /resources/site-structure
```

The mapping YAML is not a DocFX `toc.yml`, so it does not change the DocFX navigation.

## Duplicate public URLs

The sample deliberately serves the same article through two public URLs. The MVC version sets its canonical URL to the DocFX page.

```text
/resources/authentication
  canonical → /docs/guides/authentication
```

Use the canonical setting when both URLs are public and one should be treated as the primary address.

## Choosing a mapping style

Use folder mapping when an entire content area follows one URL convention, such as `/courses/**`.

Use explicit MVC mapping when only selected Markdown files need a public menu route or controller logic.

Use protected mapping when authorization must run before the article is rendered.
