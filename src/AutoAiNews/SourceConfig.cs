namespace AutoAiNews;

public sealed record SourceConfig(
    string Name,
    Uri Url,
    Uri? FeedUrl,
    int SourceWeight,
    string Mode);

