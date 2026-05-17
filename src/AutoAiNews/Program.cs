using AutoAiNews;

var options = AppOptions.FromArgs(args);
var generator = new NewsGenerator(new HttpArticleFetcher());

var result = await generator.GenerateAsync(options.ConfigPath);
var markdown = MarkdownRenderer.Render(result, DateTimeOffset.UtcNow);

Directory.CreateDirectory(options.ReportsDirectory);
var reportPath = Path.Combine(options.ReportsDirectory, $"{DateTimeOffset.UtcNow:yyyy-MM-dd}.md");

await File.WriteAllTextAsync(options.ReadmePath, markdown);
await File.WriteAllTextAsync(reportPath, markdown);

Console.WriteLine($"Wrote {options.ReadmePath}");
Console.WriteLine($"Wrote {reportPath}");

