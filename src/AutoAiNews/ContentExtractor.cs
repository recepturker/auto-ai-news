using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AutoAiNews;

public static partial class ContentExtractor
{
    public static IEnumerable<ArticleCandidate> Extract(string content, string contentType, Uri baseUrl, SourceConfig source)
    {
        if (LooksLikeXml(content, contentType))
        {
            foreach (var item in ExtractFeed(content, baseUrl, source))
            {
                yield return item;
            }

            yield break;
        }

        foreach (var item in ExtractHtml(content, baseUrl, source))
        {
            yield return item;
        }
    }

    private static IEnumerable<ArticleCandidate> ExtractFeed(string content, Uri baseUrl, SourceConfig source)
    {
        var document = XDocument.Parse(content);

        foreach (var item in document.Descendants().Where(element => element.Name.LocalName is "item" or "entry"))
        {
            var title = Clean(item.Elements().FirstOrDefault(e => e.Name.LocalName == "title")?.Value);
            var summary = Clean(item.Elements().FirstOrDefault(e => e.Name.LocalName is "description" or "summary" or "content")?.Value);
            var link = FeedLink(item);
            var published = ParseDate(item.Elements().FirstOrDefault(e => e.Name.LocalName is "pubDate" or "published" or "updated")?.Value);

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(link))
            {
                continue;
            }

            if (Uri.TryCreate(baseUrl, WebUtility.HtmlDecode(link), out var url))
            {
                yield return new ArticleCandidate(title, url, source.Name, source.SourceWeight, published, summary);
            }
        }
    }

    private static IEnumerable<ArticleCandidate> ExtractHtml(string content, Uri baseUrl, SourceConfig source)
    {
        foreach (Match match in AnchorRegex().Matches(content))
        {
            var href = WebUtility.HtmlDecode(match.Groups["href"].Value);
            var title = Clean(match.Groups["text"].Value);

            if (string.IsNullOrWhiteSpace(title) || title.Length < 8 || href.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (!Uri.TryCreate(baseUrl, href, out var url) || url.Scheme is not ("http" or "https"))
            {
                continue;
            }

            yield return new ArticleCandidate(title, url, source.Name, source.SourceWeight, null, null);
        }
    }

    private static string? FeedLink(XElement item)
    {
        var atomLink = item.Elements().FirstOrDefault(e => e.Name.LocalName == "link" && e.Attribute("href") is not null);
        if (atomLink is not null)
        {
            return atomLink.Attribute("href")?.Value;
        }

        return item.Elements().FirstOrDefault(e => e.Name.LocalName == "link")?.Value;
    }

    private static DateTimeOffset? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed)
            ? parsed
            : null;
    }

    private static bool LooksLikeXml(string content, string contentType)
    {
        return contentType.Contains("xml", StringComparison.OrdinalIgnoreCase)
            || content.TrimStart().StartsWith("<rss", StringComparison.OrdinalIgnoreCase)
            || content.TrimStart().StartsWith("<feed", StringComparison.OrdinalIgnoreCase);
    }

    private static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        var withoutTags = TagRegex().Replace(value, " ");
        return WebUtility.HtmlDecode(WhiteSpaceRegex().Replace(withoutTags, " ")).Trim();
    }

    [GeneratedRegex("<a\\s+(?:[^>]*?\\s+)?href=[\"'](?<href>[^\"']+)[\"'][^>]*>(?<text>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex AnchorRegex();

    [GeneratedRegex("<[^>]+>", RegexOptions.CultureInvariant)]
    private static partial Regex TagRegex();

    [GeneratedRegex("\\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhiteSpaceRegex();
}

