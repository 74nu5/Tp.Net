using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tickets.Api.Data;
using Tickets.Tests.Support;

namespace Tickets.Tests.Exercise03;

public class LogLevelTests
{
    private const string SqlCommandsCategory = "Microsoft.EntityFrameworkCore.Database.Command";

    private static bool IsEnabled(TicketsApiFactory factory, string category, LogLevel level) =>
        factory.Services.GetRequiredService<ILoggerFactory>().CreateLogger(category).IsEnabled(level);

    [Fact]
    public async Task InDevelopment_TheSqlCommandsAreLogged()
    {
        await using var factory = new TicketsApiFactory();
        Assert.True(IsEnabled(factory, SqlCommandsCategory, LogLevel.Information));
    }

    [Fact]
    public async Task InProduction_TheSqlCommandsAreNotLogged()
    {
        await using var factory = TicketsApiFactory.InEnvironment("Production");
        Assert.False(IsEnabled(factory, SqlCommandsCategory, LogLevel.Information));
    }

    [Fact]
    public async Task InProduction_OurOwnLogsAreStillThere()
    {
        await using var factory = TicketsApiFactory.InEnvironment("Production");
        Assert.True(IsEnabled(factory, typeof(EfTicketRepository).FullName!, LogLevel.Information));
    }
}

[Collection(DatabaseCollection.Name)]
public class StructuredLogTests : IClassFixture<TicketsApiFactory>
{
    private readonly TicketsApiFactory _factory;

    public StructuredLogTests(TicketsApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ClosingATicket_WritesAStructuredLogEntry()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = _factory.CreateClient();
        var ticket = await client.CreateTicketAsync(HttpExtensions.UniqueTitle("Logged"));
        var id = ticket["id"]!.GetValue<int>();

        (await client.PostAsync($"/tickets/{id}/close", null, ct)).EnsureSuccessStatusCode();

        // A structured entry keeps each value as a field. An interpolated string would only give the text.
        var entry = Assert.Single(_factory.Logs.GetSnapshot(), record =>
            record.Category == typeof(EfTicketRepository).FullName
            && record.StructuredState?.Any(field => field is { Key: "TicketId" } && field.Value == id.ToString(System.Globalization.CultureInfo.InvariantCulture)) == true);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains(entry.StructuredState!, field => field.Key == "OpenHours");
        Assert.StartsWith($"Ticket {id} closed", entry.Message);
    }
}
