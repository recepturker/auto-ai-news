using System.Text.RegularExpressions;

namespace AutoAiNews;

public static partial class TextUtil
{
    public static int CountVisibleWords(string htmlOrText)
    {
        var noScripts = ScriptRegex().Replace(htmlOrText, " ");
        var noStyles = StyleRegex().Replace(noScripts, " ");
        var noTags = TagRegex().Replace(noStyles, " ");
        return CountWords(noTags);
    }

    public static int CountWords(string text) => WordRegex().Matches(text).Count;

    public static string NormalizeTitle(string title)
    {
        return WhiteSpaceRegex().Replace(title.Trim().ToLowerInvariant(), " ");
    }

    [GeneratedRegex("<script\\b[^<]*(?:(?!</script>)<[^<]*)*</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex ScriptRegex();

    [GeneratedRegex("<style\\b[^<]*(?:(?!</style>)<[^<]*)*</style>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex StyleRegex();

    [GeneratedRegex("<[^>]+>", RegexOptions.CultureInvariant)]
    private static partial Regex TagRegex();

    [GeneratedRegex("[\\p{L}\\p{N}][\\p{L}\\p{N}'-]*", RegexOptions.CultureInvariant)]
    private static partial Regex WordRegex();

    [GeneratedRegex("\\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhiteSpaceRegex();
}

