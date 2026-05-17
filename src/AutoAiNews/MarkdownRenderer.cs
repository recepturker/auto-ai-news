using System.Text;

namespace AutoAiNews;

public static class MarkdownRenderer
{
    public static string Render(GeneratedReport report, DateTimeOffset generatedAt)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Auto AI News");
        builder.AppendLine();
        builder.AppendLine($"Generated: {generatedAt:yyyy-MM-dd HH:mm} UTC");
        builder.AppendLine();
        builder.AppendLine("A daily report from a strict list of curated AI sources.");
        builder.AppendLine();
        builder.AppendLine("> Transparency: this project was built with help from AI coding tools. This report is an automated aggregator; original content belongs to the linked publishers and authors.");
        builder.AppendLine();
        builder.AppendLine("## Latest Links");
        builder.AppendLine();

        if (report.Items.Count == 0)
        {
            builder.AppendLine("No AI-related links were found in this run.");
        }
        else
        {
            foreach (var item in report.Items)
            {
                builder.Append("- ");
                builder.Append(Stars(item.Stars));
                builder.Append(' ');
                builder.Append('[');
                builder.Append(EscapeLinkText(item.Title));
                builder.Append("](");
                builder.Append(item.Url);
                builder.Append(") - ");
                builder.Append(item.ReadingMinutes);
                builder.Append(" min - ");
                builder.Append(EscapeText(item.SourceName));

                if (item.PublishedAt is not null)
                {
                    builder.Append(" - ");
                    builder.Append(item.PublishedAt.Value.ToString("yyyy-MM-dd"));
                }

                builder.AppendLine();
            }
        }

        builder.AppendLine();
        builder.AppendLine("## Inaccessible Links");
        builder.AppendLine();

        if (report.Failures.Count == 0)
        {
            builder.AppendLine("No inaccessible sources were detected.");
        }
        else
        {
            foreach (var failure in report.Failures)
            {
                builder.Append("- ");
                builder.Append(EscapeText(failure.SourceName));
                builder.Append(": ");
                builder.Append(failure.Url);
                builder.Append(" - ");
                builder.AppendLine(EscapeText(failure.Reason));
            }
        }

        return builder.ToString();
    }

    private static string Stars(int count) => new string('★', count) + new string('☆', 5 - count);

    private static string EscapeText(string value) => value.ReplaceLineEndings(" ");

    private static string EscapeLinkText(string value) => value.Replace("[", "\\[").Replace("]", "\\]");
}
