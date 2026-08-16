# Request pipeline

ASP.NET Core processes each request through a sequence of middleware components.

## Middleware order

Order matters because each component can run code before and after the next component in the pipeline.

```csharp
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

## Terminal middleware

A terminal middleware component can produce a response without calling the next component.

### When to use it

Use terminal middleware for endpoints or short-circuit rules that should stop further processing.
