namespace AutoAiNews;

public sealed record ArticleCandidate(
    string Title,
    Uri Url,
    string SourceName,
    int SourceWeight,
    DateTimeOffset? PublishedAt,
    string? Summary);

