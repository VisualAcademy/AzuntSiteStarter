# Courses

Course pages use the public site header and footer while keeping documentation navigation around the article.

## Available courses

Choose a course to open its local table of contents. Each course folder owns its own `toc.yml`, so the left navigation only shows pages from the current course.

- [ASP.NET Core](/courses/aspnet-core)
- [Blazor](/courses/blazor)
- [Data access](/courses/data-access)

## How this area is organized

The Markdown files stay under `docs/Azunt.Docs/courses`. They are rendered by ASP.NET Core MVC instead of DocFX, which allows the public layout to wrap the documentation content.

The page layout has three regions:

1. local course navigation on the left
2. Markdown article content in the center
3. an `On this page` list generated from `##` and `###` headings on the right
