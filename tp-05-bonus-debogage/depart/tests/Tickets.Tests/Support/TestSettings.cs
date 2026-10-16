using Microsoft.Extensions.Configuration;

namespace Tickets.Tests.Support;

/// <summary>
/// The connection string of the test database: testsettings.json, overridden by the environment variable
/// ConnectionStrings__TicketsTests (used by the continuous integration).
/// </summary>
public static class TestSettings
{
    public static string ConnectionString { get; } =
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("testsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build()
            .GetConnectionString("TicketsTests")
        ?? throw new InvalidOperationException("No 'TicketsTests' connection string in testsettings.json.");
}
