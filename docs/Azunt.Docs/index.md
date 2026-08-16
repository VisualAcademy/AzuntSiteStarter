# Azunt documentation

Azunt.Site is an ASP.NET Core 10 solution with public, documentation, dashboard, and account shells. Course Markdown can also be rendered inside the public shell.

- **Public** — landing and public MVC pages at `/`.
- **Courses** — Markdown pages at `/courses/` using the public header and footer with local TOC navigation.
- **Docs** — DocFX 2.78.5 pages at `/docs/` with native left navigation.
- **Dashboard** — authenticated dashboard pages at `/dashboard/`.
- **Account** — sign-in, registration, and profile pages under `/account/`.

Public documentation is built from Markdown by DocFX. Protected Markdown is served through MVC after ASP.NET Core authorization.

## Start here

- [Courses](/courses)
- [Overview](/docs/overview)
- [Site structure](/docs/getting-started/site-structure)
- [Build and publish](/docs/getting-started/build-and-publish)
- [Visual Studio workflow](/docs/getting-started/development-workflow)
- [Dashboard navigation](/docs/guides/dashboard-tree-navigation)
- [Razor and CSHTML highlighting](/docs/code/razor)
- [Protected documentation](/docs/private/)

## Test account

```text
Email: demo@azunt.local
Password: Azunt123!
```

> [!NOTE]
> The test account is stored in EF Core InMemory and is cleared when the application restarts.
