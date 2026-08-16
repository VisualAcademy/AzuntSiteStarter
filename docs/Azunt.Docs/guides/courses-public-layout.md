# Courses in the public layout

The `/courses/**` area uses Markdown content with the public site header and footer.

## Source layout

Course source files live under:

```text
docs/Azunt.Docs/courses/
```

The URL follows the Markdown path.

```text
courses/aspnet-core/fundamentals/routing.md
→ /courses/aspnet-core/fundamentals/routing
```

## Local TOC selection

`MarkdownContentService` walks from the current document folder toward the course root and selects the nearest `toc.yml`.

For example:

```text
courses/aspnet-core/fundamentals/routing.md
courses/aspnet-core/fundamentals/toc.yml
```

The Fundamentals TOC is used for that page. A document in a folder without its own `toc.yml` falls back to the next parent TOC.

## MVC routes in the same path space

`CoursesController` keeps the Markdown route as a low-priority catch-all.

```csharp
[HttpGet("{*slug}", Order = 1000)]
```

A literal controller route such as `/courses/mvc-page` therefore takes precedence. This allows regular MVC pages and folder-based Markdown pages to coexist under `/courses`.

## Shared renderer

Courses no longer use a course-specific Markdown parser. `MarkdownContentService` also renders protected documentation and explicitly mapped MVC Markdown pages.
