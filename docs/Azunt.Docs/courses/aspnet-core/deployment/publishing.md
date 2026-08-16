# Publishing

`dotnet publish` creates the application files required for deployment.

## Release output

```powershell
dotnet publish -c Release
```

## Documentation content

The course Markdown and its local `toc.yml` files are copied with the web application so MVC can render them at runtime.

### Public DocFX output

The existing DocFX build remains separate and continues to publish under `/docs`.
