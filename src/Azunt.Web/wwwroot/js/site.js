document.addEventListener('click', event => {
  const trigger = event.target.closest('.nav-button');
  if (!trigger) return;
  const menu = trigger.parentElement?.querySelector('.mega-menu');
  if (!menu) return;
  menu.style.display = menu.style.display === 'grid' ? '' : 'grid';
});
