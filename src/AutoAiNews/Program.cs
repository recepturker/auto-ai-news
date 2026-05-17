using AutoAiNews;

var options = AppOptions.FromArgs(args);
var generator = new NewsGenerator(new HttpArticleFetcher());

var result = await generator.GenerateAsync(options.ConfigPath);
var markdown = MarkdownRenderer.Render(result, DateTimeOffset.UtcNow);

var outputDirectory = Path.GetDirectoryName(options.OutputPath);
if (!string.IsNullOrWhiteSpace(outputDirectory))
{
    Directory.CreateDirectory(outputDirectory);
}

await File.WriteAllTextAsync(options.OutputPath, markdown);

Console.WriteLine($"Wrote {options.OutputPath}");
