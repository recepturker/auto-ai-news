namespace AutoAiNews;

public static class Scoring
{
    public static int Stars(ArticleCandidate candidate)
    {
        var score = candidate.SourceWeight;
        score += KeywordFilter.Strength(candidate.Title, candidate.Summary);

        if (candidate.PublishedAt is not null)
        {
            var age = DateTimeOffset.UtcNow - candidate.PublishedAt.Value.ToUniversalTime();
            if (age <= TimeSpan.FromDays(2))
            {
                score += 2;
            }
            else if (age <= TimeSpan.FromDays(7))
            {
                score += 1;
            }
        }

        return Math.Clamp((int)Math.Ceiling(score / 3.0), 1, 5);
    }
}

