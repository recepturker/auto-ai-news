using AutoAiNews;

namespace AutoAiNews.Tests;

public class NewsPipelineTests
{
    [Fact]
    public void KeywordFilter_accepts_ai_related_titles()
    {
        Assert.True(KeywordFilter.IsRelevant("OpenAI releases a new reasoning model", null));
        Assert.True(KeywordFilter.IsRelevant("Better RAG evals for agent workflows", null));
        Assert.False(KeywordFilter.IsRelevant("A normal database migration story", null));
    }

    [Fact]
    public void UrlNormalizer_removes_tracking_parameters_and_fragments()
    {
        var normalized = UrlUtil.Normalize(new Uri("https://Example.com/post?utm_source=x&id=42#comments"));

        Assert.Equal("https://example.com/post?id=42", normalized);
    }

    [Fact]
    public void NewsItem_calculates_reading_minutes_from_word_count()
    {
        var candidate = new ArticleCandidate(
            "OpenAI model update",
            new Uri("https://example.com/openai"),
            "Example",
            3,
            DateTimeOffset.UtcNow,
            "large language model");

        var item = NewsItem.From(candidate, 441);

        Assert.Equal(3, item.ReadingMinutes);
        Assert.InRange(item.Stars, 1, 5);
    }

    [Fact]
    public void MarkdownRenderer_includes_links_and_failures()
    {
        var item = new NewsItem("AI [update]", new Uri("https://example.com"), "Example", 4, 5, null);
        var failure = new FetchFailure("Blocked", new Uri("https://blocked.example"), "HTTP 403 Forbidden");

        var markdown = MarkdownRenderer.Render(new GeneratedReport([item], [failure]), DateTimeOffset.Parse("2026-05-17T05:00:00Z"));

        Assert.Contains("★★★★☆ [AI \\[update\\]](https://example.com/) - 5 min - Example", markdown);
        Assert.Contains("Blocked: https://blocked.example/ - HTTP 403 Forbidden", markdown);
    }

    [Fact]
    public void ContentExtractor_reads_rss_items()
    {
        const string rss = """
        <rss><channel><item>
          <title>New AI agent benchmark</title>
          <link>https://example.com/agent</link>
          <description>Large language model evaluation details.</description>
          <pubDate>Sun, 17 May 2026 05:00:00 GMT</pubDate>
        </item></channel></rss>
        """;
        var source = new SourceConfig("Example", new Uri("https://example.com"), null, 3, "feed");

        var item = Assert.Single(ContentExtractor.Extract(rss, "application/rss+xml", source.Url, source));

        Assert.Equal("New AI agent benchmark", item.Title);
        Assert.Equal("https://example.com/agent", item.Url.ToString());
        Assert.NotNull(item.PublishedAt);
    }
}
