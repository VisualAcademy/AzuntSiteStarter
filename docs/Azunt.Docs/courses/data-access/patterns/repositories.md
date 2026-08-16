# Repository boundaries

A repository can be useful when it represents a meaningful domain boundary instead of simply wrapping every `DbSet` call.

## Keep the abstraction useful

Expose operations that match the application's use cases rather than duplicating the entire EF Core API.

## Direct DbContext use

For straightforward application code, using `DbContext` directly from an application service can be simpler and more transparent.
