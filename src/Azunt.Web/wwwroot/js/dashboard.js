(() => {
  const compactStorageKey = 'azunt.portal.dashboard.sidebar.compact.v2';
  const treeStorageKey = 'azunt.portal.dashboard.tree.classic.v2';
  const body = document.body;
  const sidebar = document.getElementById('portalSidebar');
  const topToggle = document.getElementById('sidebarToggle');
  const footerToggle = document.getElementById('sidebarFooterToggle');
  const flyout = document.getElementById('dashboardNavFlyout');
  const flyoutContent = document.getElementById('dashboardNavFlyoutContent');
  const treeState = readTreeState();

  if (localStorage.getItem(compactStorageKey) === 'true') {
    body.classList.add('sidebar-collapsed');
  }

  initializeSourceTree();
  syncToggleLabels();

  // Two separate controls:
  // - top hamburger: slide the whole sidebar in/out
  // - footer arrow: expanded sidebar <-> compact icon rail
  topToggle?.addEventListener('click', toggleSidebarVisibility);
  footerToggle?.addEventListener('click', toggleSidebarCompactness);

  document.addEventListener('click', event => {
    const toggle = event.target.closest?.('[data-nav-toggle]');
    if (toggle) {
      event.preventDefault();
      event.stopPropagation();
      toggleNode(toggle);
      return;
    }

    const link = event.target.closest?.('[data-nav-primary-link]');
    if (link && sidebar?.contains(link) && isCompactMode() && !isSidebarHidden()) {
      const node = link.closest('[data-nav-node]');
      if (node?.dataset.navDepth === '0' && node.dataset.navHasChildren === 'true') {
        event.preventDefault();
        event.stopPropagation();
        openFlyout(node, link);
        return;
      }
    }

    if (flyout?.classList.contains('is-open') && !flyout.contains(event.target) && !sidebar?.contains(event.target)) {
      closeFlyout();
    }
  });

  document.addEventListener('keydown', event => {
    if (event.key === 'Escape') closeFlyout();
  });

  window.addEventListener('resize', () => {
    if (!isCompactMode() || isSidebarHidden()) closeFlyout();
  });

  function toggleSidebarVisibility() {
    body.classList.toggle('sidebar-hidden');
    closeFlyout();
    syncToggleLabels();
  }

  function toggleSidebarCompactness() {
    const compact = body.classList.toggle('sidebar-collapsed');
    localStorage.setItem(compactStorageKey, String(compact));
    closeFlyout();
    syncToggleLabels();
  }

  function syncToggleLabels() {
    const hidden = isSidebarHidden();
    const compact = body.classList.contains('sidebar-collapsed');

    topToggle?.setAttribute('aria-expanded', String(!hidden));
    topToggle?.setAttribute('aria-label', hidden ? 'Show navigation' : 'Hide navigation');
    topToggle?.setAttribute('title', hidden ? 'Show navigation' : 'Hide navigation');

    footerToggle?.setAttribute('aria-label', compact ? 'Expand navigation' : 'Collapse navigation');
    footerToggle?.setAttribute('title', compact ? 'Expand navigation' : 'Collapse navigation');
  }

  function initializeSourceTree() {
    sidebar?.querySelectorAll('[data-nav-toggle]').forEach(button => {
      const node = button.closest('[data-nav-node]');
      if (!node) return;
      const key = node.dataset.navKey;
      const children = directChildren(node);
      if (!key || !children) return;

      const active = node.dataset.activeBranch === 'true';
      if (active) {
        setExpanded(node, button, children, true);
      } else if (Object.prototype.hasOwnProperty.call(treeState, key)) {
        setExpanded(node, button, children, treeState[key] === true);
      }
    });
  }

  function toggleNode(button) {
    const node = button.closest('[data-nav-node]');
    const children = node ? directChildren(node) : null;
    if (!node || !children) return;

    const expanded = button.getAttribute('aria-expanded') !== 'true';
    setExpanded(node, button, children, expanded);

    const key = node.dataset.navKey;
    if (key) {
      treeState[key] = expanded;
      localStorage.setItem(treeStorageKey, JSON.stringify(treeState));
      syncSourceNode(key, expanded, node);
    }
  }

  function setExpanded(node, button, children, expanded) {
    node.classList.toggle('is-expanded', expanded);
    children.classList.toggle('is-collapsed', !expanded);
    button.setAttribute('aria-expanded', String(expanded));
  }

  function directChildren(node) {
    return Array.from(node.children).find(x => x.classList?.contains('dashboard-nav-children')) ?? null;
  }

  function syncSourceNode(key, expanded, origin) {
    sidebar?.querySelectorAll(`[data-nav-node][data-nav-key="${cssEscape(key)}"]`).forEach(node => {
      if (node === origin) return;
      const button = node.querySelector(':scope > .dashboard-nav-row > [data-nav-toggle]');
      const children = directChildren(node);
      if (button && children) setExpanded(node, button, children, expanded);
    });
  }

  function openFlyout(sourceNode, trigger) {
    if (!flyout || !flyoutContent) return;

    const clone = sourceNode.cloneNode(true);
    clone.classList.add('flyout-root', 'is-expanded');
    clone.removeAttribute('id');
    clone.querySelectorAll('[id]').forEach(x => x.removeAttribute('id'));
    clone.querySelectorAll('[aria-controls]').forEach(x => x.removeAttribute('aria-controls'));

    const rootChildren = directChildren(clone);
    if (rootChildren) rootChildren.classList.remove('is-collapsed');

    flyoutContent.replaceChildren(clone);
    applyStoredState(clone);

    const rect = trigger.getBoundingClientRect();
    const maxTop = window.innerHeight - 100;
    flyout.style.top = `${Math.min(Math.max(rect.top, 56), maxTop)}px`;
    flyout.classList.add('is-open');
    flyout.setAttribute('aria-hidden', 'false');

    requestAnimationFrame(() => {
      const box = flyout.getBoundingClientRect();
      if (box.bottom > window.innerHeight - 8) {
        flyout.style.top = `${Math.max(56, window.innerHeight - box.height - 8)}px`;
      }
    });
  }

  function applyStoredState(root) {
    root.querySelectorAll('[data-nav-node]').forEach(node => {
      if (node.classList.contains('flyout-root')) return;
      const button = node.querySelector(':scope > .dashboard-nav-row > [data-nav-toggle]');
      const children = directChildren(node);
      if (!button || !children) return;

      const key = node.dataset.navKey;
      const active = node.dataset.activeBranch === 'true';
      const expanded = active || (key && Object.prototype.hasOwnProperty.call(treeState, key) ? treeState[key] === true : false);
      setExpanded(node, button, children, expanded);
    });
  }

  function closeFlyout() {
    if (!flyout || !flyoutContent) return;
    flyout.classList.remove('is-open');
    flyout.setAttribute('aria-hidden', 'true');
    flyoutContent.replaceChildren();
  }

  function isCompactMode() {
    return body.classList.contains('sidebar-collapsed') || window.matchMedia('(max-width: 820px)').matches;
  }

  function isSidebarHidden() {
    return body.classList.contains('sidebar-hidden');
  }

  function readTreeState() {
    try {
      const value = JSON.parse(localStorage.getItem(treeStorageKey) ?? '{}');
      return value && typeof value === 'object' ? value : {};
    } catch {
      return {};
    }
  }

  function cssEscape(value) {
    return window.CSS?.escape ? CSS.escape(value) : value.replace(/[^a-zA-Z0-9_-]/g, '\\$&');
  }
})();
