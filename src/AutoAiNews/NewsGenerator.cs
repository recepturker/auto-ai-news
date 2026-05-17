using System.Text.Json;
using System.Net.Sockets;

namespace AutoAiNews;

public sealed class NewsGenerator(IArticleFetcher fetcher)
{
    private const int MaxCandidatesPerSource = 25;
    private const int MaxItems = 30;
    private static readonly TimeSpan MaxPublishedAge = TimeSpan.FromDays(14);

    public async Task<GeneratedReport> GenerateAsync(string configPath, CancellationToken cancellationToken = default)
    {
        var config = await SourceConfigFile.LoadAsync(configPath, cancellationToken);
        var candidates = new List<ArticleCandidate>();
        var failures = new List<FetchFailure>();

        foreach (var source in config.Sources)
        {
            var fetchUrl = source.FeedUrl ?? source.Url;

            try
            {
                var result = await fetcher.FetchAsync(fetchUrl, cancellationToken);
                var extracted = ContentExtractor.Extract(result.Content, result.ContentType, result.FinalUrl, source)
                    .Take(MaxCandidatesPerSource);
                candidates.AddRange(extracted);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or InvalidOperationException)
            {
                failures.Add(new FetchFailure(source.Name, fetchUrl, FailureReason(ex)));
            }
        }

        var items = new List<NewsItem>();
        foreach (var candidate in Deduplicate(candidates).Where(IsFresh).Where(c => KeywordFilter.IsRelevant(c.Title, c.Summary)))
        {
            var wordCount = EstimateWords(candidate.Title, candidate.Summary);

            try
            {
                var article = await fetcher.FetchAsync(candidate.Url, cancellationToken);
                wordCount = Math.Max(wordCount, TextUtil.CountVisibleWords(article.Content));
            }
            catch
            {
                // Source pages are still useful even when article body fetches fail.
            }

            items.Add(NewsItem.From(candidate, wordCount));
        }

        var ranked = items
            .OrderByDescending(item => item.Stars)
            .ThenByDescending(item => item.PublishedAt ?? DateTimeOffset.MinValue)
            .ThenBy(item => item.Title, StringComparer.OrdinalIgnoreCase)
            .Take(MaxItems)
            .ToList();

        return new GeneratedReport(ranked, failures);
    }

    private static IEnumerable<ArticleCandidate> Deduplicate(IEnumerable<ArticleCandidate> candidates)
    {
        var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var seenTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var candidate in candidates)
        {
            var normalizedUrl = UrlUtil.Normalize(candidate.Url);
            var normalizedTitle = TextUtil.NormalizeTitle(candidate.Title);

            if (string.IsNullOrWhiteSpace(normalizedTitle))
            {
                continue;
            }

            if (seenUrls.Add(normalizedUrl) && seenTitles.Add(normalizedTitle))
            {
                yield return candidate;
            }
        }
    }

    private static int EstimateWords(string title, string? summary) => Math.Max(80, TextUtil.CountWords($"{title} {summary}"));

    private static bool IsFresh(ArticleCandidate candidate)
    {
        return candidate.PublishedAt is null || DateTimeOffset.UtcNow - candidate.PublishedAt.Value.ToUniversalTime() <= MaxPublishedAge;
    }

    private static string FailureReason(Exception ex)
    {
        if (ex is TaskCanceledException)
        {
            return "Request timed out";
        }

        if (ex is HttpRequestException { InnerException: SocketException socket })
        {
            return $"Connection failed: {socket.SocketErrorCode}";
        }

        return ex.Message;
    }
}

public sealed record SourceConfigFile(IReadOnlyList<SourceConfig> Sources)
{
    public static async Task<SourceConfigFile> LoadAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var config = await JsonSerializer.DeserializeAsync<SourceConfigFile>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        if (config is null || config.Sources.Count == 0)
        {
            throw new InvalidOperationException($"No sources found in {path}");
        }

        return config;
    }
}
