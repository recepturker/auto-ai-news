using System.Web;

namespace AutoAiNews;

public static class UrlUtil
{
    public static string Normalize(Uri url)
    {
        var builder = new UriBuilder(url)
        {
            Fragment = "",
            Scheme = url.Scheme.ToLowerInvariant(),
            Host = url.Host.ToLowerInvariant()
        };

        var query = HttpUtility.ParseQueryString(builder.Query);
        foreach (var key in query.AllKeys.Where(IsTrackingKey).ToList())
        {
            query.Remove(key);
        }

        builder.Query = query.ToString();
        return builder.Uri.ToString().TrimEnd('/');
    }

    private static bool IsTrackingKey(string? key)
    {
        return key is not null
            && (key.StartsWith("utm_", StringComparison.OrdinalIgnoreCase)
                || key.Equals("fbclid", StringComparison.OrdinalIgnoreCase)
                || key.Equals("gclid", StringComparison.OrdinalIgnoreCase));
    }
}

