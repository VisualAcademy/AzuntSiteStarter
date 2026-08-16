# Routing

Routing matches an incoming URL to an endpoint in the application.

## Conventional routing

MVC can use a conventional route for common controller and action patterns.

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

## Attribute routing

Attribute routes keep the URL contract close to the controller action.

```csharp
[HttpGet("/courses/{*slug}")]
public IActionResult Page(string? slug) => View();
```

### Choosing a style

Use the style that makes the route contract easiest to understand and maintain.
