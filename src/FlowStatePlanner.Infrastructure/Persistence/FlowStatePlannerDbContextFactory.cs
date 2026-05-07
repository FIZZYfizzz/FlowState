using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FlowStatePlanner.Infrastructure.Persistence;

public sealed class FlowStatePlannerDbContextFactory : IDesignTimeDbContextFactory<FlowStatePlannerDbContext>
{
    public FlowStatePlannerDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? Environment.GetEnvironmentVariable("FLOWSTATEPLANNER_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Connection string 'Postgres' was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<FlowStatePlannerDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "planner"));

        return new FlowStatePlannerDbContext(optionsBuilder.Options);
    }

    private static IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var apiSettingsDirectory = FindApiSettingsDirectory(Directory.GetCurrentDirectory());

        return new ConfigurationBuilder()
            .SetBasePath(apiSettingsDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
    }

    private static string FindApiSettingsDirectory(string currentDirectory)
    {
        var candidates = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, "src", "FlowStatePlanner.Api"),
            Path.Combine(currentDirectory, "..", "FlowStatePlanner.Api"),
            Path.Combine(currentDirectory, "..", "..", "src", "FlowStatePlanner.Api")
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(Path.Combine(fullPath, "appsettings.json")))
            {
                return fullPath;
            }
        }

        return currentDirectory;
    }
}
