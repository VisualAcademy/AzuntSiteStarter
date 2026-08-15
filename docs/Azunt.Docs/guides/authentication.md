# Authentication

The web project uses ASP.NET Core Identity with the EF Core InMemory provider. SQL Server is not required for local testing.

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("Azunt.Site.Identity.Demo"));
```

## Test account

The application seeds this account at startup:

```text
Email: demo@azunt.local
Password: Azunt123!
```

> [!NOTE]
> InMemory data is cleared when the application process restarts. Use SQL Server or Azure SQL for persistent identity storage.

Dashboard routes and protected docs use the same authentication cookie and AccountShell.
