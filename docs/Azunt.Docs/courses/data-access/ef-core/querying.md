# Querying

LINQ queries are translated by the database provider where possible.

## Read-only queries

Use `AsNoTracking` when retrieved entities do not need change tracking.

```csharp
var items = await db.Products
    .AsNoTracking()
    .OrderBy(x => x.Name)
    .ToListAsync();
```

## Projection

Project only the fields the page needs when a full entity is unnecessary.
