using System.Net;
using System.Net.Http.Json;
using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise08;

/// <summary>
/// Exercise 08: integration tests written by the apprentice. This is one possible answer.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class MyApiTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    // The whole pipeline runs: routing, validation, the repository and SQL Server. No mock involved.
    [Fact]
    public async Task ACreatedTicket_AppearsInTheSummary()
    {
        var client = factory.CreateClient();
        var title = HttpExtensions.UniqueTitle("Summary check");
        var id = (await client.CreateTicketAsync(title))["id"]!.GetValue<int>();

        var summary = await (await client.GetAsync("/tickets/summary", TestContext.Current.CancellationToken)).ReadJsonAsync();

        Assert.Contains(summary.AsArray(), line => line!["id"]!.GetValue<int>() == id && line["commentCount"]!.GetValue<int>() == 0);
    }

    [Fact]
    public async Task AnUnknownPriority_IsRejected()
    {
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/tickets", new { title = "Bad priority", priority = "Urgent" }, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
