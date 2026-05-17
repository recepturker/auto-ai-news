# Auto AI News

Daily AI news reports generated from a strict list of curated sources.

Reports are published as GitHub Releases so each daily report is easy to find from the repository homepage and GitHub mobile app.

## Sources

The source list lives in `config/sources.json` and currently uses:

- LessWrong AI
- Substack Technology
- Latent Space AI News
- The Rundown AI
- Simon Willison
- smol.ai News
- Frontier AI Lab Tracker

## Local Usage

```powershell
dotnet run --project src\AutoAiNews\AutoAiNews.csproj -- --output generated\daily-report.md
```

