using System.Net;
using System.Net.Http.Json;
using Tickets.Tests.Support;

namespace Tickets.Tests.Exercise02;

public class StackTraceAnswerTests
{
    [Fact]
    public void TheFailingMethod_IsTheOneTheStackTraceShowed() =>
        Assert.Equal("0931a0c09cd2fe45", AnswerCheck.Hash(Answers.StatsFailingMethod));
}

[Collection(DatabaseCollection.Name)]
public class StatsTests : IClassFixture<TicketsApiFactory>
{
    private readonly HttpClient _client;

    public StatsTests(TicketsApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Stats_CountTheTicketsAndTheComments()
    {
        var ct = TestContext.Current.CancellationToken;
        // A closed ticket, so that this test does not depend on the third anomaly of exercise 06.
        var closed = await _client.CreateTicketAsync(HttpExtensions.UniqueTitle("Stats closed"));
        (await _client.PostAsync($"/tickets/{closed["id"]}/close", null, ct)).EnsureSuccessStatusCode();
        var open = await _client.CreateTicketAsync(HttpExtensions.UniqueTitle("Stats open"));
        (await _client.PostAsJsonAsync($"/tickets/{open["id"]}/comments", new { author = "alice", text = "Still there" }, ct)).EnsureSuccessStatusCode();

        var response = await _client.GetAsync("/tickets/stats", ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stats = await response.ReadJsonAsync();
        Assert.True(stats["open"]!.GetValue<int>() >= 1);
        Assert.True(stats["closed"]!.GetValue<int>() >= 1);
        Assert.True(stats["comments"]!.GetValue<int>() >= 1);
    }
}
