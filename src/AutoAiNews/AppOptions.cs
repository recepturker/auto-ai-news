namespace AutoAiNews;

public sealed record AppOptions(string ConfigPath, string OutputPath)
{
    public static AppOptions FromArgs(string[] args)
    {
        var configPath = "config/sources.json";
        var outputPath = "generated/daily-report.md";

        for (var i = 0; i < args.Length; i++)
        {
            if (i + 1 >= args.Length)
            {
                continue;
            }

            switch (args[i])
            {
                case "--config":
                    configPath = args[++i];
                    break;
                case "--output":
                    outputPath = args[++i];
                    break;
            }
        }

        return new AppOptions(configPath, outputPath);
    }
}
