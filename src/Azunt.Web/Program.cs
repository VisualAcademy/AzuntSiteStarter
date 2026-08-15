using Azunt.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("Azunt.Site.Identity.Demo"));

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/access-denied";
    options.ReturnUrlParameter = "returnUrl";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/home/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Keep public DocFX URLs clean and independent from the documentation source folder.
// DocFX still emits .html files, but callers use /docs/overview instead of
// /docs/overview.html. Protected MVC docs are excluded from this rewrite.
app.Use(async (context, next) =>
{
    var requestPath = context.Request.Path;
    var path = requestPath.Value ?? string.Empty;
    var isPublicDocs = requestPath.StartsWithSegments("/docs") &&
                       !requestPath.StartsWithSegments("/docs/private");

    if (isPublicDocs && path.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
    {
        var cleanPath = path[..^5];
        if (cleanPath.EndsWith("/index", StringComparison.OrdinalIgnoreCase))
        {
            cleanPath = cleanPath[..^6] + "/";
        }

        context.Response.Redirect(cleanPath + context.Request.QueryString, permanent: false);
        return;
    }

    if (isPublicDocs &&
        path.Length > "/docs".Length &&
        !path.EndsWith('/') &&
        string.IsNullOrEmpty(Path.GetExtension(path)))
    {
        context.Request.Path = path + ".html";
    }

    await next();
});

// Public web assets and published DocFX output.
app.UseDefaultFiles();
app.UseStaticFiles();

// During local development/build, DocFX output stays under obj instead of polluting wwwroot.
var localDocFxPath = Path.Combine(app.Environment.ContentRootPath, "obj", "DocFxSite");
if (Directory.Exists(localDocFxPath))
{
    var docsProvider = new PhysicalFileProvider(localDocFxPath);

    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = docsProvider,
        RequestPath = "/docs"
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = docsProvider,
        RequestPath = "/docs"
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await SeedData.InitializeAsync(app.Services);

app.Run();
