using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Api.Data;

namespace Tickets.Api.Tests.Support;

/// <summary>
/// Starts the whole API in memory, as in production but without a real network port.
/// Points it to the test database and counts the SQL commands it sends.
/// </summary>
public class TicketsApiFactory : WebApplicationFactory<Program>
{
    private readonly Dictionary<string, string?> _settings;

    public TicketsApiFactory() : this([]) { }

    private TicketsApiFactory(Dictionary<string, string?> settings) => _settings = settings;

    public SqlCommandCounter SqlCommands { get; } = new();

    /// <summary>A new factory with extra configuration values, e.g. ("Tickets:PageSize", "2").</summary>
    public static TicketsApiFactory With(params (string Key, string? Value)[] settings) =>
        new(settings.ToDictionary(s => s.Key, s => s.Value));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configuration =>
        {
            // Added last, so these values win over appsettings.json, user-secrets and environment variables.
            var values = new Dictionary<string, string?> { ["ConnectionStrings:Tickets"] = TestSettings.ConnectionString };
            foreach (var (key, value) in _settings)
            {
                values[key] = value;
            }

            configuration.AddInMemoryCollection(values);
        });

        builder.ConfigureServices(services =>
            services.ConfigureDbContext<TicketsDbContext>(options => options.AddInterceptors(SqlCommands)));
    }
}

/// <summary>Counts the SQL commands EF Core sends, to spot N+1 queries.</summary>
public sealed class SqlCommandCounter : DbCommandInterceptor
{
    private int _count;

    public int Count => _count;

    public void Reset() => Interlocked.Exchange(ref _count, 0);

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _count);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _count);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }
}
