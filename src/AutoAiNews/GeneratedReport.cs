namespace AutoAiNews;

public sealed record GeneratedReport(IReadOnlyList<NewsItem> Items, IReadOnlyList<FetchFailure> Failures);

