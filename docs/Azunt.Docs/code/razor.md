# Razor and CSHTML syntax highlighting

The custom DocFX template registers both `razor` and `cshtml` language names with Highlight.js.

## Razor component

```razor
@page "/counter"
@using System.Globalization

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>
<button class="btn" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

## MVC Razor view

```cshtml
@model ProductViewModel
@{
    ViewData["Title"] = "Products";
}

<h1>@ViewData["Title"]</h1>

@foreach (var product in Model.Products)
{
    <article class="product-card">
        <h2>@product.Name</h2>
        <a asp-controller="Products"
           asp-action="Details"
           asp-route-id="@product.Id">Open</a>
    </article>
}
```

## C# remains normal

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

The included Razor grammar covers directives, expressions, HTML tags, strings, comments, and common C# keywords. It can be replaced with another Highlight.js grammar without changing the Markdown source.
