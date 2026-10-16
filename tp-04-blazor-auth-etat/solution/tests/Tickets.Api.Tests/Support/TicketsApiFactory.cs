using System.Data.Common;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
/// Points it to the test database, counts the SQL commands it sends, and trusts the tokens of <see cref="TestTokens"/>.
/// <see cref="CreateClient()"/> calls the API as an administrator; use <see cref="CreateClientFor"/> or
/// <see cref="CreateAnonymousClient"/> to test authentication and authorization.
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
        {
            services.ConfigureDbContext<TicketsDbContext>(options => options.AddInterceptors(SqlCommands));

            // PostConfigure runs after the API's own configuration: it adds the test issuer, audience and key.
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidIssuer = TestTokens.Issuer;
                options.TokenValidationParameters.ValidAudience = TestTokens.Audience;
                options.TokenValidationParameters.IssuerSigningKey = TestTokens.SigningKey;
            });
        });
    }

    // Every client from CreateClient() is an administrator, so that the tests of TP-02 keep working once writes need a token.
    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.For("test-admin", "admin"));
    }

    public HttpClient CreateClientFor(string name, params string[] roles)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.For(name, roles));
        return client;
    }

    public HttpClient CreateAnonymousClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = null;
        return client;
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
