using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Tickets.Api.Domain;
using Tickets.Api.Statistics;
using Tickets.Tests.Support;

namespace Tickets.Tests.Exercise06;

/// <summary>Anomaly 1: « Je modifie un ticket, le site me dit OK, mais rien ne change. »</summary>
[Collection(DatabaseCollection.Name)]
public class Anomaly1Tests : IClassFixture<TicketsApiFactory>
{
    private readonly HttpClient _client;

    public Anomaly1Tests(TicketsApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task UpdatingATicket_SavesTheChanges()
    {
        var ct = TestContext.Current.CancellationToken;
        var ticket = await _client.CreateTicketAsync(HttpExtensions.UniqueTitle("Before"));
        var newTitle = HttpExtensions.UniqueTitle("After");

        var response = await _client.PutAsJsonAsync($"/tickets/{ticket["id"]}", new { title = newTitle, description = "Edited", priority = "High" }, ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var saved = await (await _client.GetAsync($"/tickets/{ticket["id"]}", ct)).ReadJsonAsync();
        Assert.Equal(newTitle, saved["title"]!.GetValue<string>());
        Assert.Equal("High", saved["priority"]!.GetValue<string>());
    }
}

/// <summary>Anomaly 2: « Le résumé des tickets ne montre pas le ticket que je viens de créer. »</summary>
[Collection(DatabaseCollection.Name)]
public class Anomaly2Tests : IClassFixture<TicketsApiFactory>
{
    private readonly HttpClient _client;

    public Anomaly2Tests(TicketsApiFactory factory) => _client = factory.CreateClient();

    private async Task<JsonArray> GetSummaryAsync() =>
        (await (await _client.GetAsync("/tickets/summary", TestContext.Current.CancellationToken)).ReadJsonAsync()).AsArray();

    [Fact]
    public async Task TheSummary_ShowsANewTicket()
    {
        await GetSummaryAsync();   // fills the cache
        var title = HttpExtensions.UniqueTitle("Fresh");

        await _client.CreateTicketAsync(title);

        Assert.Contains(await GetSummaryAsync(), summary => summary!["title"]!.GetValue<string>() == title);
    }

    [Fact]
    public async Task TheSummary_ShowsAClosedTicketAsClosed()
    {
        var ticket = await _client.CreateTicketAsync(HttpExtensions.UniqueTitle("To close"));
        await GetSummaryAsync();   // fills the cache

        (await _client.PostAsync($"/tickets/{ticket["id"]}/close", null, TestContext.Current.CancellationToken)).EnsureSuccessStatusCode();

        var summary = Assert.Single(await GetSummaryAsync(), summary => summary!["id"]!.GetValue<int>() == ticket["id"]!.GetValue<int>());
        Assert.Equal("Closed", summary!["status"]!.GetValue<string>());
    }
}

/// <summary>Anomaly 3: « Sur la base neuve de la recette, la page des statistiques plante. »</summary>
public class Anomaly3Tests
{
    private static readonly DateTimeOffset Monday = new(2026, 10, 19, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void WithoutClosedTickets_ThereIsNoAverage()
    {
        var stats = TicketStatistics.Compute([new TicketDates(TicketStatus.Open, Monday, null)], comments: 0);

        Assert.Equal(new TicketStats(Open: 1, Closed: 0, Comments: 0, AverageResolutionHours: null), stats);
    }

    [Fact]
    public void WithoutAnyTicket_EverythingIsZero() =>
        Assert.Equal(new TicketStats(0, 0, 0, null), TicketStatistics.Compute([], comments: 0));

    [Fact]
    public void TheAverage_IsInHours()
    {
        var stats = TicketStatistics.Compute(
            [
                new TicketDates(TicketStatus.Closed, Monday, Monday.AddHours(2)),
                new TicketDates(TicketStatus.Closed, Monday, Monday.AddHours(4)),
                new TicketDates(TicketStatus.Open, Monday, null),
            ],
            comments: 3);

        Assert.Equal(new TicketStats(Open: 1, Closed: 2, Comments: 3, AverageResolutionHours: 3), stats);
    }
}
