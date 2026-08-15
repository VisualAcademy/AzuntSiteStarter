# Visual Studio development workflow

Keep the ASP.NET Core process running while rebuilding documentation.

## First run

Open `Azunt.Site.slnx`, select `Azunt.Web` as the startup project, and run once:

```text
Ctrl + F5
```

## Edit Markdown

After changing Markdown, `toc.yml`, `docfx.json`, or DocFX template files:

```text
Ctrl + Shift + B
```

Then refresh the browser.

```text
Edit docs
   ↓
Ctrl + Shift + B
   ↓
DocFX 2.78.5 rebuild
   ↓
obj/DocFxSite updated
   ↓
Refresh browser
```

There is no need to restart the web server for a documentation-only change.

## Visual Studio change tracking

`Azunt.Web.csproj` registers the DocFX source tree with the fast up-to-date check:

```xml
<UpToDateCheckInput Include="$(DocFxRoot)/**/*.md" Set="DocFx" />
<UpToDateCheckInput Include="$(DocFxRoot)/**/*.yml" Set="DocFx" />
<UpToDateCheckInput Include="$(DocFxRoot)/docfx.json" Set="DocFx" />
<UpToDateCheckInput Include="$(DocFxRoot)/templates/**/*" Set="DocFx" />
<UpToDateCheckOutput Include="$(DocFxBuildStamp)" Set="DocFx" />
```

Build Output should contain:

```text
Building Azunt public documentation with DocFX 2.78.5...
```

## Action by file type

| Changed file | Action |
| --- | --- |
| `*.md` | `Ctrl + Shift + B`, then refresh |
| `toc.yml` | `Ctrl + Shift + B`, then refresh |
| `docfx.json` | `Ctrl + Shift + B`, then refresh |
| DocFX CSS / JavaScript | `Ctrl + Shift + B`, then hard refresh if needed |
| C# / MVC / Razor application code | Use normal build, Hot Reload, F5, or restart as needed |

## SelfHostWebServer duplicate-key error

Some Visual Studio builds can show this message when an already-running ASP.NET Core project is started again:

```text
An element with the same key but a different value already exists.
Key: 'Microsoft.WebTools.ProjectSystem.WebServer.SelfHostWebServer'
```

For documentation changes, leave the current web process running and rebuild instead of starting it again.

If the error is already present:

1. Stop the application with `Shift + F5` or the Stop button.
2. Start it once.
3. If needed, close and reopen the solution.
4. If needed, restart Visual Studio.
5. As a fallback, run `Run.cmd` or use `dotnet run` from a terminal.

```powershell
dotnet run --project .\src\Azunt.Web\Azunt.Web.csproj --launch-profile https
```

## Daily loop

```text
Ctrl + F5 once
   ↓
Edit docs
   ↓
Ctrl + Shift + B
   ↓
Refresh /docs/...
```
