namespace AutoAiNews;

public interface IArticleFetcher
{
    Task<FetchResult> FetchAsync(Uri url, CancellationToken cancellationToken);
}

public sealed class HttpArticleFetcher : IArticleFetcher
{
    private readonly HttpClient _client;

    public HttpArticleFetcher()
    {
        _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20),
            MaxResponseContentBufferSize = 4 * 1024 * 1024
        };
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("auto-ai-news/1.0 (+https://github.com/recepturker/auto-ai-news)");
        _client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/rss+xml,application/atom+xml,application/xml;q=0.9,*/*;q=0.5");
    }

    public async Task<FetchResult> FetchAsync(Uri url, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
        return new FetchResult(content, contentType, response.RequestMessage?.RequestUri ?? url);
    }
}

