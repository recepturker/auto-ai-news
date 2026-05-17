namespace AutoAiNews;

public sealed record AppOptions(string ConfigPath, string ReadmePath, string ReportsDirectory)
{
    public static AppOptions FromArgs(string[] args)
    {
        var configPath = "config/sources.json";
        var readmePath = "README.md";
        var reportsDirectory = "reports";

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
                case "--readme":
                    readmePath = args[++i];
                    break;
                case "--reports":
                    reportsDirectory = args[++i];
                    break;
            }
        }

        return new AppOptions(configPath, readmePath, reportsDirectory);
    }
}

