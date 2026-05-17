# Auto AI News

Daily AI news reports generated from a strict list of curated sources.

Reports are published as GitHub Releases so each daily report is easy to find from the repository homepage and GitHub mobile app.

## Transparency

This project was built with help from AI coding tools. The generated reports are automated aggregations of links from the configured sources.

Original content belongs to the linked publishers and authors. Auto AI News does not republish their articles; it only lists links, titles, estimated reading time, and simple ranking metadata to help readers discover the original work.

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

