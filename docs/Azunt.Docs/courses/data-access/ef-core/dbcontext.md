# DbContext

`DbContext` represents a unit of work with the database and tracks entity changes.

## Registration

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

## Lifetime

A scoped lifetime aligns naturally with one web request in most MVC applications.
