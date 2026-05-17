namespace AutoAiNews;

public sealed record NewsItem(
    string Title,
    Uri Url,
    string SourceName,
    int Stars,
    int ReadingMinutes,
    DateTimeOffset? PublishedAt)
{
    public static NewsItem From(ArticleCandidate candidate, int wordCount)
    {
        return new NewsItem(
            candidate.Title,
            candidate.Url,
            candidate.SourceName,
            Scoring.Stars(candidate),
            Math.Max(1, (int)Math.Ceiling(wordCount / 220.0)),
            candidate.PublishedAt);
    }
}

