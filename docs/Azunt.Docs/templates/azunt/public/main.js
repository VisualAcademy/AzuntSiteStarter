import razor from './razor.js'

function attachShellLinks() {
  if (document.querySelector('.azunt-shell-links')) return

  const host = document.querySelector(
    'header .container-xxl, header .container-fluid, .navbar .container-xxl, .navbar .container-fluid'
  )

  if (!host) return

  const links = document.createElement('nav')
  links.className = 'azunt-shell-links'
  links.setAttribute('aria-label', 'Azunt site navigation')
  links.innerHTML = `
    <a href="/">Public</a>
    <a href="/courses">Courses</a>
    <a class="active" href="/docs/">Docs</a>
    <a href="/dashboard">Dashboard</a>
    <a href="/account/profile">Account</a>`

  host.appendChild(links)
}

export default {
  defaultTheme: 'light',

  configureHljs(hljs) {
    hljs.registerLanguage('razor', razor)
    hljs.registerLanguage('cshtml', razor)
  },

  start() {
    attachShellLinks()
    window.setTimeout(attachShellLinks, 0)
  }
}
