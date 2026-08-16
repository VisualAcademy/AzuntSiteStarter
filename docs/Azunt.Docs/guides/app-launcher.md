# App launcher

The waffle button in the dashboard header opens the Azunt app launcher. It provides quick access to the main site areas without leaving the current shell first.

## Included links

- Public site
- Dashboard
- Resources
- Documentation
- Protected docs
- Account
- Products
- Pricing

The panel also includes shortcuts to resources, getting started documentation, and the account profile.

## Files

The launcher markup is in `Views/Shared/_DashboardLayout.cshtml`.

Its styles are in `wwwroot/css/dashboard.css`, and open/close behavior is handled by `wwwroot/js/dashboard.js`.

The launcher icons use symbols from `wwwroot/images/dashboard-icons.svg`.

## Adding an item

Add another `.portal-app-item` link to the `portal-app-grid` block in `_DashboardLayout.cshtml`. Reuse an existing SVG symbol or add a new symbol to `dashboard-icons.svg`, then assign a background class for the tile in `dashboard.css`.
