namespace Azunt.Web.Models.Shells;

// Navigation entries for the Fluid sample only. Neither the standard Landing
// header nor its mega-menu data depends on this catalog.
public sealed record FluidNavLink(string Title, string Description, string Url);
public sealed record FluidNavGroup(string Title, string Description, IReadOnlyList<FluidNavLink> Links);
public sealed record FluidNavItem(string Id, string Title, IReadOnlyList<FluidNavLink> Links, IReadOnlyList<FluidNavGroup> Groups);

public static class FluidNavigation
{
    // MVC / Razor Pages and Blazor Fluid headers render this same catalog.
    // Destinations deliberately match the corresponding original mega menus.
    public static IReadOnlyList<FluidNavItem> Items { get; } =
    [
        new("explore", "Explore",
            [
                new("Platform overview", "Explore the Azunt starter", "/products"),
                new("Cloud dashboard", "Operations and workspace", "/dashboard"),
                new("Documentation", "Guides and implementation notes", "/docs/"),
                new("Create an account", "Get started with Azunt", "/account/register"),
            ], []),
        new("products", "Products",
            [new("All products", "Browse product areas", "/products")],
            [
                new("Platform", "Public website and product pages",
                    [
                        new("Application platform", "Reusable site structure", "/products"),
                        new("Landing experience", "Container landing page", "/"),
                        new("Pricing pages", "Plans and subscriptions", "/pricing"),
                    ]),
                new("Content & learning", "Learn and build",
                    [
                        new("Course library", "Structured courses", "/courses"),
                        new("Resource center", "Technical content", "/resources"),
                        new("Developer docs", "DocFX documentation", "/docs/"),
                    ]),
                new("Operations", "Manage and monitor",
                    [
                        new("Dashboard", "Operations workspace", "/dashboard"),
                        new("Resources", "Manage app resources", "/dashboard/resources"),
                        new("Monitoring", "Service health", "/dashboard/monitoring"),
                    ]),
                new("Identity", "Account experience",
                    [
                        new("Sign in", "Authentication", "/account/login"),
                        new("Account profile", "Personal settings", "/account/profile"),
                        new("Register", "Create a new account", "/account/register"),
                    ]),
            ]),
        new("solutions", "Solutions",
            [new("Solutions overview", "Explore building blocks", "/products")],
            [
                new("Use cases", "Popular site patterns",
                    [
                        new("SaaS starter", "Branded products", "/products"),
                        new("Internal portal", "Secure operations", "/dashboard"),
                        new("Developer portal", "Docs and reference", "/docs/"),
                        new("Learning hub", "Courses and training", "/courses"),
                    ]),
                new("For teams", "Patterns for every team",
                    [
                        new("Product teams", "Consistent user experience", "/products"),
                        new("Engineering teams", "Guides and architecture", "/docs/"),
                        new("Operations teams", "Monitoring workspace", "/dashboard/monitoring"),
                        new("Training teams", "Structured learning", "/courses"),
                    ]),
                new("Architecture", "Extendable foundations",
                    [
                        new("Site structure", "Source boundaries", "/resources/site-structure"),
                        new("Authentication", "Identity-ready starter", "/resources/authentication"),
                        new("Dashboard", "Secure administration", "/dashboard"),
                    ]),
            ]),
        new("pricing", "Pricing",
            [
                new("Plans & pricing", "Compare commercial options", "/pricing"),
                new("Start free", "Register a new account", "/account/register"),
                new("Open dashboard", "See the application shell", "/dashboard"),
            ], []),
        new("ecosystem", "Ecosystem",
            [
                new("Developer docs", "Integration-ready guidance", "/docs/"),
                new("Technical resources", "Implementation examples", "/resources"),
                new("Learning content", "Courses and walkthroughs", "/courses"),
            ], []),
        new("resources", "Resources",
            [new("Resource center", "Browse all resources", "/resources")],
            [
                new("Learn", "Education and references",
                    [
                        new("Courses", "Structured courses", "/courses"),
                        new("Resource library", "Curated technical content", "/resources"),
                    ]),
                new("Build", "Development guidance",
                    [
                        new("Documentation", "DocFX guides", "/docs/"),
                        new("Routing guide", "MVC and site routing", "/resources/routing"),
                    ]),
                new("Operate", "Authenticated experiences",
                    [
                        new("Dashboard", "Manage resources", "/dashboard"),
                        new("Service health", "Operations monitoring", "/dashboard/monitoring"),
                    ]),
                new("Account", "Sign in or join",
                    [
                        new("Sign in", "Access your account", "/account/login"),
                        new("Create account", "Register for Azunt", "/account/register"),
                    ]),
            ]),
        new("company", "Company",
            [
                new("About Azunt Starter", "Starter introduction", "/"),
                new("Architecture", "Product areas", "/products"),
                new("Site structure", "Solution boundaries", "/resources/site-structure"),
                new("Commercialize", "Plans and pricing", "/pricing"),
            ], []),
        new("more", "More",
            [
                new("Getting started", "Setup and implementation guides", "/docs/"),
                new("Plans & pricing", "Commercial page starter", "/pricing"),
            ],
            [
                new("Templates", "Ready-to-adapt site experiences",
                    [
                        new("Landing (Container)", "Centered layout", "/"),
                        new("Landing (Fluid) · MVC", "Full-width MVC demo", "/samples/mvc/landing-fluid"),
                        new("Landing (Fluid) · Razor", "Full-width Razor Pages demo", "/samples/razor/landing-fluid"),
                        new("Landing (Fluid) · Blazor", "Full-width Blazor demo", "/samples/blazor/landing-fluid"),
                        new("Product pages", "Features and products", "/products"),
                        new("Dashboard", "Operations workspace", "/dashboard"),
                        new("Documentation", "Developer docs", "/docs/"),
                    ]),
                new("Examples", "Starter content patterns",
                    [
                        new("Courses", "Structured learning", "/courses"),
                        new("Resource center", "Curated resources", "/resources"),
                        new("Routing guide", "MVC routing", "/resources/routing"),
                    ]),
            ]),
    ];
}
