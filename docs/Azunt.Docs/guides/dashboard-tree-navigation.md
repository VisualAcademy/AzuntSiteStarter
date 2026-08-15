# Dashboard navigation

The dashboard uses a dark multi-level sidebar with two separate controls.

## Controls

- **Top hamburger** — slides the entire sidebar out of view or brings it back.
- **Bottom arrow** — switches between the full sidebar and the compact icon rail.

The two states are independent. Hiding the sidebar gives the content the full page width. Compact mode keeps a narrow icon rail visible.

## Tree behavior

- Parent nodes can contain nested nodes.
- The active branch opens automatically.
- Full mode shows icons, labels, indentation, and chevrons.
- Compact mode shows top-level icons.
- Clicking a parent icon in compact mode opens a dark flyout tree.
- Flyout list markers are removed.
- Flyout branches can contain additional levels.
- Clicking outside the flyout or pressing `Esc` closes it.
- Compact/full state and branch expansion are stored in `localStorage`.
- The top-hamburger hidden state is not persisted.

Navigation data is defined in `DashboardNavigationViewComponent.cs`. `_DashboardNavNode.cshtml` renders the tree recursively.
