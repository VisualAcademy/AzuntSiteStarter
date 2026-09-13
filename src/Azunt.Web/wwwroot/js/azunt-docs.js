(() => {
  const escapeHtml = value => value
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;');

  const razorKeywords = /\b(if|else|for|foreach|while|switch|case|break|continue|return|new|var|class|public|private|protected|internal|static|async|await|using|namespace|true|false|null|try|catch|finally|throw)\b/g;
  const directives = /^@(page|model|using|inject|inherits|implements|layout|namespace|attribute|section|code|functions)\b/gm;

  document.querySelectorAll('code.language-razor, code.language-cshtml').forEach(code => {
    let html = escapeHtml(code.textContent ?? '');
    html = html.replace(/(@\*[^]*?\*@)/g, '<span class="rz-comment">$1</span>');
    html = html.replace(/(&lt;\/?[A-Za-z][^&]*?&gt;)/g, '<span class="rz-tag">$1</span>');
    html = html.replace(directives, '<span class="rz-directive">@$1</span>');
    html = html.replace(razorKeywords, '<span class="rz-keyword">$1</span>');
    html = html.replace(/(&quot;[^&]*?&quot;)/g, '<span class="rz-string">$1</span>');
    html = html.replace(/(@[A-Za-z_][A-Za-z0-9_.]*)/g, '<span class="rz-expression">$1</span>');
    code.innerHTML = html;
  });
})();
