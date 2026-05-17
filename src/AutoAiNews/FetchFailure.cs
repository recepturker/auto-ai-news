namespace AutoAiNews;

public sealed record FetchFailure(string SourceName, Uri Url, string Reason);

