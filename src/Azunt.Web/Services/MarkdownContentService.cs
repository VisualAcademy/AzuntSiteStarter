using System.Net;
using System.Text.RegularExpressions;
using Azunt.Web.Models.Docs;
using Azunt.Web.Models.Markdown;
using Markdig;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Azunt.Web.Services;

public sealed partial class MarkdownContentService
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private readonly IDeserializer _yamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private readonly string _outputRoot = Path.GetFullPath(AppContext.BaseDirectory);

    public Task<MarkdownPageViewModel?> GetFolderPageAsync(MarkdownFolderPageRequest request)
    {
        var root = ResolveContentRoot(request.ContentRoot);
        if (root is null)
        {
            return Task.FromResult<MarkdownPageViewModel?>(null);
        }

        var slug = NormalizeSlug(request.Slug, request.DefaultSlug);
        if (!SafeSlugRegex().IsMatch(slug))
        {
            return Task.FromResult<MarkdownPageViewModel?>(null);
        }

        var filePath = ResolveMarkdownPath(root, slug);
        if (filePath is null)
        {
            return Task.FromResult<MarkdownPageViewModel?>(null);
        }

        var currentUrl = ToUrl(filePath, root, request.BaseUrl);

        return BuildPageAsync(
            root,
            filePath,
            currentUrl,
            request.BaseUrl,
            request.RootTitle,
            request.BaseUrl,
            request.TocMode,
            request.TocPath,
            request.NavigationTitle,
            request.CanonicalUrl,
            request.BuildSourceBreadcrumbs);
    }

    public Task<MarkdownPageViewModel?> GetFilePageAsync(MarkdownFilePageRequest request)
    {
        var root = ResolveContentRoot(request.ContentRoot);
        if (root is null)
        {
            return Task.FromResult<MarkdownPageViewModel?>(null);
        }

        var relativePath = request.MarkdownPath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        var filePath = SafeCombine(root, relativePath, root);
        if (filePath is null || !filePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase) || !File.Exists(filePath))
        {
            return Task.FromResult<MarkdownPageViewModel?>(null);
        }

        var rootUrl = string.IsNullOrWhiteSpace(request.RootUrl) ? request.CurrentUrl : request.RootUrl;

        return BuildPageAsync(
            root,
            filePath,
            NormalizeUrl(request.CurrentUrl),
            NormalizeUrl(request.NavigationBaseUrl),
            request.RootTitle,
            rootUrl,
            request.TocMode,
            request.TocPath,
            request.NavigationTitle,
            request.CanonicalUrl,
            request.BuildSourceBreadcrumbs);
    }

    private async Task<MarkdownPageViewModel?> BuildPageAsync(
        string root,
        string filePath,
        string currentUrl,
        string navigationBaseUrl,
        string rootTitle,
        string? rootUrl,
        MarkdownTocMode tocMode,
        string? explicitTocPath,
        string? navigationTitleOverride,
        string? canonicalUrl,
        bool buildSourceBreadcrumbs)
    {
        if (!File.Exists(filePath) || !IsUnderRoot(filePath, root))
        {
            return null;
        }

        var markdown = await File.ReadAllTextAsync(filePath);
        var parsed = ParseFrontMatter(markdown);
        var title = parsed.Title ?? ExtractTitle(parsed.Markdown) ?? Humanize(Path.GetFileNameWithoutExtension(filePath));
        var html = Markdown.ToHtml(parsed.Markdown, MarkdownPipeline);

        var tocPath = ResolveTocPath(root, filePath, tocMode, explicitTocPath);
        var navigation = tocPath is null
            ? Array.Empty<MarkdownNavigationItemViewModel>()
            : await BuildNavigationAsync(root, tocPath, currentUrl, navigationBaseUrl);

        var navigationTitle = !string.IsNullOrWhiteSpace(navigationTitleOverride)
            ? navigationTitleOverride
            : tocMode == MarkdownTocMode.Nearest && tocPath is not null
                ? await ResolveNavigationTitleAsync(root, Path.GetDirectoryName(tocPath)!, rootTitle)
                : rootTitle;

        var breadcrumbs = buildSourceBreadcrumbs
            ? await BuildSourceBreadcrumbsAsync(root, filePath, title, currentUrl, navigationBaseUrl, rootTitle, rootUrl)
            : BuildSimpleBreadcrumbs(title, currentUrl, rootTitle, rootUrl);

        return new MarkdownPageViewModel
        {
            Title = title,
            Html = html,
            Slug = currentUrl.Trim('/'),
            SourcePath = Path.GetRelativePath(root, filePath).Replace('\\', '/'),
            RootTitle = rootTitle,
            RootUrl = rootUrl,
            NavigationTitle = navigationTitle,
            Navigation = navigation,
            OnThisPage = ExtractHeadings(html),
            Breadcrumbs = breadcrumbs,
            CanonicalUrl = canonicalUrl
        };
    }

    private string? ResolveContentRoot(string contentRoot)
    {
        if (string.IsNullOrWhiteSpace(contentRoot))
        {
            return null;
        }

        var root = Path.GetFullPath(Path.Combine(_outputRoot, contentRoot));
        var outputPrefix = _outputRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!root.Equals(_outputRoot, StringComparison.OrdinalIgnoreCase) &&
            !root.StartsWith(outputPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return Directory.Exists(root) ? root : null;
    }

    private static string? ResolveMarkdownPath(string root, string slug)
    {
        var relative = slug == "index" ? "index" : slug;
        var candidate = SafeCombine(root, relative.Replace('/', Path.DirectorySeparatorChar) + ".md", root);
        if (candidate is not null && File.Exists(candidate))
        {
            return candidate;
        }

        candidate = SafeCombine(root, Path.Combine(relative.Replace('/', Path.DirectorySeparatorChar), "index.md"), root);
        return candidate is not null && File.Exists(candidate) ? candidate : null;
    }

    private string? ResolveTocPath(string root, string filePath, MarkdownTocMode tocMode, string? explicitTocPath)
    {
        return tocMode switch
        {
            MarkdownTocMode.None => null,
            MarkdownTocMode.Nearest => FindNearestToc(root, filePath),
            MarkdownTocMode.Explicit when !string.IsNullOrWhiteSpace(explicitTocPath) =>
                ResolveExplicitToc(root, explicitTocPath),
            _ => null
        };
    }

    private static string? ResolveExplicitToc(string root, string tocPath)
    {
        var candidate = SafeCombine(root, tocPath.Replace('/', Path.DirectorySeparatorChar), root);
        return candidate is not null && File.Exists(candidate) ? candidate : null;
    }

    private static string? FindNearestToc(string root, string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        while (!string.IsNullOrEmpty(directory) && IsUnderRoot(directory, root))
        {
            var toc = Path.Combine(directory, "toc.yml");
            if (File.Exists(toc))
            {
                return toc;
            }

            if (Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar)
                .Equals(root.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        return null;
    }

    private async Task<IReadOnlyList<MarkdownNavigationItemViewModel>> BuildNavigationAsync(
        string root,
        string tocPath,
        string currentUrl,
        string navigationBaseUrl)
    {
        var yaml = await File.ReadAllTextAsync(tocPath);
        var items = _yamlDeserializer.Deserialize<List<MarkdownTocItem>>(yaml) ?? [];
        var tocDirectory = Path.GetDirectoryName(tocPath)!;

        return items
            .Select(item => BuildNavigationItem(root, item, tocDirectory, currentUrl, navigationBaseUrl))
            .ToArray();
    }

    private MarkdownNavigationItemViewModel BuildNavigationItem(
        string root,
        MarkdownTocItem item,
        string tocDirectory,
        string currentUrl,
        string navigationBaseUrl)
    {
        var href = ResolveTocHref(root, item.Href, tocDirectory, navigationBaseUrl);
        var children = (item.Items ?? [])
            .Select(child => BuildNavigationItem(root, child, tocDirectory, currentUrl, navigationBaseUrl))
            .ToArray();

        var active = href is not null && UrlEquals(StripAnchor(href), StripAnchor(currentUrl));
        var activePath = active || children.Any(child => child.IsInActivePath);

        return new MarkdownNavigationItemViewModel
        {
            Name = string.IsNullOrWhiteSpace(item.Name) ? "Untitled" : item.Name.Trim(),
            Href = href,
            IsActive = active,
            IsInActivePath = activePath,
            Items = children
        };
    }

    private static string? ResolveTocHref(string root, string? href, string tocDirectory, string navigationBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return null;
        }

        href = href.Trim();
        if (href.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            href.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            href.StartsWith('/'))
        {
            return href;
        }

        var anchorIndex = href.IndexOf('#');
        var anchor = anchorIndex >= 0 ? href[anchorIndex..] : string.Empty;
        var pathPart = anchorIndex >= 0 ? href[..anchorIndex] : href;

        if (string.IsNullOrEmpty(pathPart))
        {
            return anchor;
        }

        var candidate = SafeCombine(tocDirectory, pathPart.Replace('/', Path.DirectorySeparatorChar), root);
        if (candidate is null)
        {
            return null;
        }

        if (Path.GetExtension(candidate).Equals(".md", StringComparison.OrdinalIgnoreCase) && File.Exists(candidate))
        {
            return ToUrl(candidate, root, navigationBaseUrl) + anchor;
        }

        if (Directory.Exists(candidate))
        {
            var indexPath = Path.Combine(candidate, "index.md");
            if (File.Exists(indexPath))
            {
                return ToUrl(indexPath, root, navigationBaseUrl) + anchor;
            }
        }

        return null;
    }

    private static async Task<string> ResolveNavigationTitleAsync(string root, string tocDirectory, string rootTitle)
    {
        var indexPath = Path.Combine(tocDirectory, "index.md");
        if (File.Exists(indexPath))
        {
            var markdown = await File.ReadAllTextAsync(indexPath);
            var parsed = ParseFrontMatter(markdown);
            var title = parsed.Title ?? ExtractTitle(parsed.Markdown);
            if (!string.IsNullOrWhiteSpace(title))
            {
                return title;
            }
        }

        if (Path.GetFullPath(tocDirectory).TrimEnd(Path.DirectorySeparatorChar)
            .Equals(root.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
        {
            return rootTitle;
        }

        return Humanize(Path.GetFileName(tocDirectory));
    }

    private static async Task<IReadOnlyList<MarkdownBreadcrumbViewModel>> BuildSourceBreadcrumbsAsync(
        string root,
        string filePath,
        string pageTitle,
        string currentUrl,
        string baseUrl,
        string rootTitle,
        string? rootUrl)
    {
        var breadcrumbs = new List<MarkdownBreadcrumbViewModel>
        {
            new()
            {
                Name = rootTitle,
                Href = !string.IsNullOrWhiteSpace(rootUrl) && !UrlEquals(currentUrl, rootUrl) ? rootUrl : null
            }
        };

        var fileDirectory = Path.GetDirectoryName(filePath)!;
        var relativeDirectory = Path.GetRelativePath(root, fileDirectory);
        var parts = relativeDirectory == "."
            ? Array.Empty<string>()
            : relativeDirectory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

        var currentDirectory = root;
        foreach (var part in parts)
        {
            currentDirectory = Path.Combine(currentDirectory, part);
            var indexPath = Path.Combine(currentDirectory, "index.md");
            var label = Humanize(part);

            if (File.Exists(indexPath))
            {
                var indexMarkdown = await File.ReadAllTextAsync(indexPath);
                var parsed = ParseFrontMatter(indexMarkdown);
                label = parsed.Title ?? ExtractTitle(parsed.Markdown) ?? label;
            }

            var url = File.Exists(indexPath) ? ToUrl(indexPath, root, baseUrl) : null;
            breadcrumbs.Add(new MarkdownBreadcrumbViewModel
            {
                Name = label,
                Href = url is not null && !UrlEquals(url, currentUrl) ? url : null
            });
        }

        if (!Path.GetFileName(filePath).Equals("index.md", StringComparison.OrdinalIgnoreCase))
        {
            breadcrumbs.Add(new MarkdownBreadcrumbViewModel { Name = pageTitle });
        }

        return breadcrumbs;
    }

    private static IReadOnlyList<MarkdownBreadcrumbViewModel> BuildSimpleBreadcrumbs(
        string pageTitle,
        string currentUrl,
        string rootTitle,
        string? rootUrl)
    {
        var breadcrumbs = new List<MarkdownBreadcrumbViewModel>
        {
            new()
            {
                Name = rootTitle,
                Href = !string.IsNullOrWhiteSpace(rootUrl) && !UrlEquals(currentUrl, rootUrl) ? rootUrl : null
            }
        };

        if (string.IsNullOrWhiteSpace(rootUrl) || !UrlEquals(currentUrl, rootUrl))
        {
            breadcrumbs.Add(new MarkdownBreadcrumbViewModel { Name = pageTitle });
        }

        return breadcrumbs;
    }

    private static string ToUrl(string filePath, string root, string baseUrl)
    {
        if (!IsUnderRoot(filePath, root))
        {
            return NormalizeUrl(baseUrl);
        }

        var relative = Path.GetRelativePath(root, Path.GetFullPath(filePath)).Replace('\\', '/');
        if (relative.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            relative = relative[..^3];
        }

        if (relative.Equals("index", StringComparison.OrdinalIgnoreCase))
        {
            return NormalizeUrl(baseUrl);
        }

        if (relative.EndsWith("/index", StringComparison.OrdinalIgnoreCase))
        {
            relative = relative[..^6];
        }

        return NormalizeUrl(baseUrl) + "/" + relative.Trim('/');
    }

    private static ParsedMarkdown ParseFrontMatter(string markdown)
    {
        using var reader = new StringReader(markdown);
        if (!string.Equals(reader.ReadLine()?.Trim(), "---", StringComparison.Ordinal))
        {
            return new ParsedMarkdown(markdown, null);
        }

        var metadataLines = new List<string>();
        string? line;
        var closed = false;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.Equals(line.Trim(), "---", StringComparison.Ordinal))
            {
                closed = true;
                break;
            }

            metadataLines.Add(line);
        }

        if (!closed)
        {
            return new ParsedMarkdown(markdown, null);
        }

        string? title = null;
        foreach (var metadataLine in metadataLines)
        {
            var separator = metadataLine.IndexOf(':');
            if (separator <= 0)
            {
                continue;
            }

            var key = metadataLine[..separator].Trim();
            if (!key.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            title = metadataLine[(separator + 1)..].Trim().Trim('"', '\'');
            break;
        }

        return new ParsedMarkdown(reader.ReadToEnd(), string.IsNullOrWhiteSpace(title) ? null : title);
    }

    private static string? ExtractTitle(string markdown)
    {
        using var reader = new StringReader(markdown);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                return line[2..].Trim();
            }
        }

        return null;
    }

    private static IReadOnlyList<DocsHeadingViewModel> ExtractHeadings(string html)
    {
        var headings = new List<DocsHeadingViewModel>();
        foreach (Match match in HeadingRegex().Matches(html))
        {
            var id = match.Groups["id"].Value;
            if (string.IsNullOrWhiteSpace(id))
            {
                continue;
            }

            var text = HtmlTagRegex().Replace(match.Groups["text"].Value, string.Empty);
            text = WebUtility.HtmlDecode(text).Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            headings.Add(new DocsHeadingViewModel
            {
                Id = id,
                Text = text,
                Level = int.Parse(match.Groups["level"].Value)
            });
        }

        return headings;
    }

    private static string Humanize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Content";
        }

        var text = value.Replace('-', ' ').Replace('_', ' ');
        return string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }

    private static string NormalizeSlug(string? slug, string defaultSlug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return string.IsNullOrWhiteSpace(defaultSlug) ? "index" : defaultSlug.Trim().Trim('/');
        }

        return slug.Trim().Trim('/');
    }

    private static string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || url == "/")
        {
            return "/";
        }

        return "/" + url.Trim().Trim('/');
    }

    private static string StripAnchor(string value)
    {
        var index = value.IndexOf('#');
        return index >= 0 ? value[..index] : value;
    }

    private static bool IsUnderRoot(string path, string root)
    {
        var fullPath = Path.GetFullPath(path);
        var rootPath = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar);
        var rootPrefix = rootPath + Path.DirectorySeparatorChar;
        return fullPath.Equals(rootPath, StringComparison.OrdinalIgnoreCase) ||
               fullPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private static string? SafeCombine(string root, string relative, string allowedRoot)
    {
        var combined = Path.GetFullPath(Path.Combine(root, relative));
        return IsUnderRoot(combined, allowedRoot) ? combined : null;
    }

    private static bool UrlEquals(string left, string right) =>
        string.Equals(left.TrimEnd('/'), right.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex("^[a-zA-Z0-9/_-]+$")]
    private static partial Regex SafeSlugRegex();

    [GeneratedRegex("<h(?<level>[23])\\s+id=\"(?<id>[^\"]+)\"[^>]*>(?<text>.*?)</h\\k<level>>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex HeadingRegex();

    [GeneratedRegex("<[^>]+>", RegexOptions.Singleline)]
    private static partial Regex HtmlTagRegex();

    private sealed record ParsedMarkdown(string Markdown, string? Title);

    private sealed class MarkdownTocItem
    {
        public string Name { get; set; } = string.Empty;
        public string? Href { get; set; }
        public List<MarkdownTocItem>? Items { get; set; }
    }
}
