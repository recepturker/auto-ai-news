using System.Text.RegularExpressions;

namespace AutoAiNews;

public static partial class KeywordFilter
{
    private static readonly string[] StrongKeywords =
    [
        "artificial intelligence",
        "machine learning",
        "deep learning",
        "large language model",
        "generative ai",
        "foundation model",
        "frontier model",
        "openai",
        "anthropic",
        "deepmind",
        "llm",
        "agentic",
        "inference",
        "transformer"
    ];

    private static readonly string[] WeakKeywords =
    [
        "ai",
        "model",
        "agent",
        "agents",
        "prompt",
        "embedding",
        "rag",
        "token",
        "eval",
        "gpu",
        "nvidia",
        "copilot"
    ];

    public static bool IsRelevant(string title, string? summary)
    {
        var text = $"{title} {summary}".ToLowerInvariant();
        return StrongKeywords.Any(text.Contains) || WeakKeywords.Count(keyword => WholeWord(keyword).IsMatch(text)) >= 2;
    }

    public static int Strength(string title, string? summary)
    {
        var text = $"{title} {summary}".ToLowerInvariant();
        var strong = StrongKeywords.Count(text.Contains);
        var weak = WeakKeywords.Count(keyword => WholeWord(keyword).IsMatch(text));
        return Math.Min(5, strong * 2 + weak);
    }

    private static Regex WholeWord(string word) => new($@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
}

