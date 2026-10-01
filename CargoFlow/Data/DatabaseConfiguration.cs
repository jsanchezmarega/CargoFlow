using DotNetEnv;

namespace CargoFlow.Data;

public static class DatabaseConfiguration
{
    public static string GetConnectionString()
    {
        Env.TraversePath().Load();

        var host = GetRequiredEnvironmentVariable("DB_HOST");
        var port = GetRequiredEnvironmentVariable("DB_PORT");
        var database = GetRequiredEnvironmentVariable("DB_NAME");
        var user = GetRequiredEnvironmentVariable("DB_USER");
        var password = GetRequiredEnvironmentVariable("DB_PASSWORD");

        return
            $"Server={host},{port};" +
            $"Database={database};" +
            $"User Id={user};" +
            $"Password={password};" +
            $"TrustServerCertificate=True";
    }

    private static string GetRequiredEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name)
            ?? throw new InvalidOperationException($"{name} is not set.");
    }
}