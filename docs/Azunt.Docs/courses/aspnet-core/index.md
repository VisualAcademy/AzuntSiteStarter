# ASP.NET Core

This course groups the core pieces used to build an ASP.NET Core web application.

## What you will cover

You will work through request processing, routing, authentication, and publishing. The examples are intentionally small so each page can be used as a reference later.

## Course structure

Use the navigation on the left to move between topics. Only the `aspnet-core/toc.yml` tree is shown while you are inside this course.

## Sample command

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.MapDefaultControllerRoute();
app.Run();
```
