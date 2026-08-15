# Build and publish

`Azunt.Web.csproj` runs DocFX 2.78.5 with .NET 10 `dotnet tool exec` before the web build completes.

```powershell
dotnet build .\src\Azunt.Web\Azunt.Web.csproj
```

Development output:

```text
src/Azunt.Web/obj/DocFxSite
```

The application serves that folder at `/docs` while developing.

## Publish

```powershell
dotnet publish .\src\Azunt.Web\Azunt.Web.csproj -c Release
```

DocFX output is included in the publish artifact under:

```text
wwwroot/docs/**
```

DocFX is not required on the deployed server.

## Skip the docs build

```powershell
dotnet build .\src\Azunt.Web\Azunt.Web.csproj -p:BuildDocsOnBuild=false
```
