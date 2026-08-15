export default function razor(hljs) {
  const csharpKeywords = {
    className: 'keyword',
    begin: /\b(?:abstract|as|async|await|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|record|ref|required|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|var|virtual|void|volatile|while|with|yield)\b/
  }

  const razorDirective = {
    className: 'meta',
    begin: /^\s*@(?:page|model|using|inject|inherits|implements|layout|namespace|attribute|typeparam|section|code|functions|addTagHelper|removeTagHelper|tagHelperPrefix)\b/,
    end: /$/
  }

  const razorComment = {
    className: 'comment',
    begin: /@\*/,
    end: /\*@/
  }

  const razorExpression = {
    className: 'variable',
    begin: /@[A-Za-z_][A-Za-z0-9_.]*/
  }

  const razorTransition = {
    className: 'symbol',
    begin: /@(?=\{|\(|:)/,
    relevance: 4
  }

  const htmlTag = {
    className: 'tag',
    begin: /<\/?[A-Za-z][A-Za-z0-9:-]*/,
    end: /\/?>/,
    contains: [
      {
        className: 'attr',
        begin: /[A-Za-z_:][-A-Za-z0-9_:.]*(?=\s*=)/
      },
      {
        className: 'string',
        variants: [
          { begin: /\"/, end: /\"/, contains: [razorExpression] },
          { begin: /'/, end: /'/, contains: [razorExpression] }
        ]
      },
      razorExpression
    ]
  }

  return {
    name: 'Razor',
    aliases: ['cshtml'],
    contains: [
      razorComment,
      razorDirective,
      htmlTag,
      {
        className: 'string',
        variants: [
          { begin: /@?\"/, end: /\"/, contains: [razorExpression] },
          { begin: /'/, end: /'/ }
        ]
      },
      csharpKeywords,
      razorExpression,
      razorTransition,
      hljs.C_LINE_COMMENT_MODE,
      hljs.C_BLOCK_COMMENT_MODE,
      hljs.NUMBER_MODE
    ]
  }
}
