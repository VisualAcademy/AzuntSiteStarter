(() => {
  'use strict';

  const header = document.querySelector('[data-az-header]');
  if (!header) return;

  const primaryNav = header.querySelector('[data-az-primary-nav]');
  const navToggle = header.querySelector('[data-az-nav-toggle]');
  const scrim = header.querySelector('[data-az-menu-scrim]');
  const menuItems = Array.from(header.querySelectorAll('[data-az-menu-item]'));
  const menuTriggers = Array.from(header.querySelectorAll('[data-az-menu-trigger]'));
  const desktopMedia = window.matchMedia('(min-width: 961px)');

  const isDesktop = () => desktopMedia.matches;

  const getPanel = trigger => {
    const id = trigger?.getAttribute('aria-controls');
    return id ? document.getElementById(id) : null;
  };

  const resetNestedTabs = item => {
    item?.querySelectorAll('[data-az-tabs]').forEach(tabs => {
      tabs.classList.remove('is-mobile-tab-open');
    });
  };

  const closeMenu = (item, restoreFocus = false) => {
    if (!item) return;

    const trigger = item.querySelector('[data-az-menu-trigger]');
    const panel = trigger ? getPanel(trigger) : null;

    item.classList.remove('is-open');
    trigger?.setAttribute('aria-expanded', 'false');
    if (panel) panel.hidden = true;
    resetNestedTabs(item);

    if (restoreFocus) trigger?.focus();
  };

  const syncHeaderState = () => {
    const anyOpen = menuItems.some(item => item.classList.contains('is-open'));
    header.classList.toggle('has-open-menu', anyOpen && isDesktop());
    primaryNav?.classList.toggle('is-submenu-open', anyOpen && !isDesktop());
  };

  const closeAllMenus = (exceptItem = null) => {
    menuItems.forEach(item => {
      if (item !== exceptItem) closeMenu(item);
    });
    syncHeaderState();
  };

  const openMenu = item => {
    const trigger = item?.querySelector('[data-az-menu-trigger]');
    const panel = trigger ? getPanel(trigger) : null;
    if (!trigger || !panel) return;

    closeAllMenus(item);
    item.classList.add('is-open');
    trigger.setAttribute('aria-expanded', 'true');
    panel.hidden = false;
    resetNestedTabs(item);
    syncHeaderState();

    if (!isDesktop()) {
      primaryNav?.scrollTo({ top: 0, behavior: 'auto' });
      const back = panel.querySelector('[data-az-menu-back]');
      window.requestAnimationFrame(() => back?.focus({ preventScroll: true }));
    }
  };

  menuTriggers.forEach(trigger => {
    trigger.addEventListener('click', event => {
      event.preventDefault();
      const item = trigger.closest('[data-az-menu-item]');
      const isOpen = item?.classList.contains('is-open');

      if (isOpen) {
        closeMenu(item);
        syncHeaderState();
      } else {
        openMenu(item);
      }
    });
  });

  header.querySelectorAll('[data-az-menu-back]').forEach(button => {
    button.addEventListener('click', () => {
      const item = button.closest('[data-az-menu-item]');
      closeMenu(item, true);
      syncHeaderState();
      primaryNav?.scrollTo({ top: 0, behavior: 'auto' });
    });
  });

  const setMobileNav = (open, restoreFocus = false) => {
    if (!primaryNav || !navToggle) return;

    primaryNav.classList.toggle('is-open', open);
    navToggle.setAttribute('aria-expanded', String(open));
    navToggle.setAttribute('aria-label', open ? 'Close navigation' : 'Open navigation');
    primaryNav.setAttribute('aria-hidden', String(!open && !isDesktop()));
    document.documentElement.classList.toggle('az-mobile-nav-open', open);
    document.body.classList.toggle('az-nav-open', open);

    if (!open) {
      closeAllMenus();
      primaryNav.scrollTop = 0;
      if (restoreFocus) navToggle.focus();
    } else {
      window.requestAnimationFrame(() => {
        const firstTrigger = primaryNav.querySelector('[data-az-menu-trigger]');
        firstTrigger?.focus({ preventScroll: true });
      });
    }
  };

  navToggle?.addEventListener('click', () => {
    setMobileNav(!primaryNav?.classList.contains('is-open'));
  });

  scrim?.addEventListener('click', () => closeAllMenus());

  document.addEventListener('click', event => {
    if (!isDesktop()) return;
    if (!header.contains(event.target)) closeAllMenus();
  });

  document.addEventListener('keydown', event => {
    if (event.key !== 'Escape') return;

    const openItem = menuItems.find(item => item.classList.contains('is-open'));
    if (openItem) {
      const openTabs = openItem.querySelector('[data-az-tabs].is-mobile-tab-open');
      if (!isDesktop() && openTabs) {
        openTabs.classList.remove('is-mobile-tab-open');
        openTabs.querySelector('[role="tab"][aria-selected="true"]')?.focus();
        return;
      }

      closeMenu(openItem, true);
      syncHeaderState();
      return;
    }

    if (!isDesktop() && primaryNav?.classList.contains('is-open')) {
      setMobileNav(false, true);
    }
  });

  const createMobileTabHeading = (panel, menuLabel, tabLabel, tabs) => {
    let heading = panel.querySelector('.az-mobile-tab-heading');
    if (heading) {
      heading.querySelector('strong').textContent = tabLabel;
      return heading;
    }

    heading = document.createElement('div');
    heading.className = 'az-mobile-tab-heading';
    heading.innerHTML = `
      <button type="button" class="az-mobile-tab-back" aria-label="Back to ${menuLabel}">
        <svg aria-hidden="true"><use href="#az-i-back"></use></svg>
        <span>${menuLabel}</span>
      </button>
      <strong>${tabLabel}</strong>`;

    heading.querySelector('.az-mobile-tab-back').addEventListener('click', () => {
      tabs.classList.remove('is-mobile-tab-open');
      primaryNav?.scrollTo({ top: 0, behavior: 'auto' });
      tabs.querySelector('[role="tab"][aria-selected="true"]')?.focus({ preventScroll: true });
    });

    panel.prepend(heading);
    return heading;
  };

  header.querySelectorAll('[data-az-tabs]').forEach(tabs => {
    tabs.classList.add('az-tabs-mobile');
    const buttons = Array.from(tabs.querySelectorAll('[role="tab"]'));
    const ownerItem = tabs.closest('[data-az-menu-item]');
    const ownerTrigger = ownerItem?.querySelector('[data-az-menu-trigger]');
    const menuLabel = ownerTrigger?.textContent.trim().replace(/\s+/g, ' ') || 'Menu';

    const activateTab = (button, focus = false, enterMobilePanel = false) => {
      buttons.forEach(tab => {
        const selected = tab === button;
        tab.classList.toggle('is-active', selected);
        tab.setAttribute('aria-selected', String(selected));
        tab.tabIndex = selected ? 0 : -1;

        const panelId = tab.getAttribute('aria-controls');
        const panel = panelId ? document.getElementById(panelId) : null;
        if (panel) panel.hidden = !selected;
      });

      const panelId = button.getAttribute('aria-controls');
      const activePanel = panelId ? document.getElementById(panelId) : null;

      if (!isDesktop() && enterMobilePanel && activePanel) {
        createMobileTabHeading(activePanel, menuLabel, button.textContent.trim(), tabs);
        tabs.classList.add('is-mobile-tab-open');
        primaryNav?.scrollTo({ top: 0, behavior: 'auto' });
        window.requestAnimationFrame(() => {
          activePanel.querySelector('.az-mobile-tab-back')?.focus({ preventScroll: true });
        });
      }

      if (focus) button.focus();
    };

    buttons.forEach((button, index) => {
      button.addEventListener('click', () => activateTab(button, false, true));
      button.addEventListener('keydown', event => {
        if (!['ArrowRight', 'ArrowLeft', 'ArrowDown', 'ArrowUp', 'Home', 'End', 'Enter', ' '].includes(event.key)) return;

        if ((event.key === 'Enter' || event.key === ' ') && !isDesktop()) {
          event.preventDefault();
          activateTab(button, false, true);
          return;
        }

        if (event.key === 'Enter' || event.key === ' ') return;
        event.preventDefault();

        let nextIndex = index;
        if (event.key === 'Home') nextIndex = 0;
        else if (event.key === 'End') nextIndex = buttons.length - 1;
        else if (event.key === 'ArrowRight' || event.key === 'ArrowDown') nextIndex = (index + 1) % buttons.length;
        else nextIndex = (index - 1 + buttons.length) % buttons.length;

        activateTab(buttons[nextIndex], true, false);
      });
    });
  });

  const syncViewportMode = () => {
    closeAllMenus();
    document.documentElement.classList.remove('az-mobile-nav-open');
    document.body.classList.remove('az-nav-open');

    if (!primaryNav || !navToggle) return;

    if (isDesktop()) {
      primaryNav.classList.remove('is-open');
      primaryNav.removeAttribute('aria-hidden');
      navToggle.setAttribute('aria-expanded', 'false');
      navToggle.setAttribute('aria-label', 'Open navigation');
    } else {
      primaryNav.classList.remove('is-open');
      primaryNav.setAttribute('aria-hidden', 'true');
      navToggle.setAttribute('aria-expanded', 'false');
      navToggle.setAttribute('aria-label', 'Open navigation');
    }
  };

  if (typeof desktopMedia.addEventListener === 'function') {
    desktopMedia.addEventListener('change', syncViewportMode);
  } else {
    desktopMedia.addListener(syncViewportMode);
  }

  syncViewportMode();
})();
