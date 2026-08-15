# ASP.NET Core routing

Static DocFX output and dynamic MVC routes can share the `/docs` URL prefix because the public static-file middleware only handles files that actually exist. A request such as `/docs/private/` falls through to endpoint routing and reaches the authorized MVC controller.

```csharp
[Authorize]
[Route("docs/private")]
public sealed class DocsController : Controller
{
    [HttpGet("")]
    [HttpGet("{*slug}")]
    public async Task<IActionResult> Private(string? slug)
    {
        // Read protected Markdown outside wwwroot and render after authorization.
    }
}
```

This route separates public static documentation from authenticated MVC documentation.
