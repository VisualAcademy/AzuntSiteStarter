# Authentication

Authentication establishes who is using the application. Authorization decides what that user can access.

## Cookie authentication

ASP.NET Core Identity commonly uses an application cookie for browser-based applications.

## Authorization policies

Policies are useful when access depends on more than a role name.

```csharp
[Authorize(Policy = "CustomerDocumentation")]
public IActionResult InternalGuide() => View();
```

### Course pages

The sample `courses` area is public. The existing `/docs/private` area remains protected by ASP.NET Core authorization.
