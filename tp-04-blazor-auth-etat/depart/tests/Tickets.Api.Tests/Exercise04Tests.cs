using System.Net;
using System.Net.Http.Json;
using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise04;

[Collection(DatabaseCollection.Name)]
public class TicketCrudTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private CancellationToken Token => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Post_CreatesATicket_AndPointsToIt()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/tickets", new { title = "Printer on fire", priority = "High" }, Token);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.ReadJsonAsync();
        Assert.Equal("Printer on fire", created["title"]!.GetValue<string>());
        Assert.Equal("High", created["priority"]!.GetValue<string>());
        Assert.Equal("Open", created["status"]!.GetValue<string>());
        Assert.EndsWith($"/tickets/{created["id"]}", response.Headers.Location?.OriginalString ?? "");
    }

    [Fact]
    public async Task Get_ReturnsTheTicket()
    {
        var client = factory.CreateClient();
        var created = await client.CreateTicketAsync("Screen flickers");

        var response = await client.GetAsync($"/tickets/{created["id"]}", Token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Screen flickers", (await response.ReadJsonAsync())["title"]!.GetValue<string>());
    }

    [Fact]
    public async Task Get_UnknownTicket_IsA404ProblemDetails()
    {
        var response = await factory.CreateClient().GetAsync("/tickets/999999", Token);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData("")]
    [InlineData("This title is far too long. This title is far too long. This title is far too long. This title is far too long. This title is far too long. This title is far too long. This title is far too long. This title is far too long.")]
    public async Task Post_InvalidTitle_IsA400WithTheFieldInError(string title)
    {
        var response = await factory.CreateClient().PostAsJsonAsync("/tickets", new { title }, Token);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await response.ReadJsonAsync())["errors"]!.AsObject();
        Assert.Contains(errors, error => error.Key.Equals("title", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Put_ReplacesTheTicket()
    {
        var client = factory.CreateClient();
        var created = await client.CreateTicketAsync("Keyboard missing a key");

        var response = await client.PutAsJsonAsync($"/tickets/{created["id"]}", new { title = "Keyboard missing the Q key", priority = "Low" }, Token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await (await client.GetAsync($"/tickets/{created["id"]}", Token)).ReadJsonAsync();
        Assert.Equal("Keyboard missing the Q key", updated["title"]!.GetValue<string>());
        Assert.Equal("Low", updated["priority"]!.GetValue<string>());
    }

    [Fact]
    public async Task Put_UnknownTicket_IsA404()
    {
        var response = await factory.CreateClient().PutAsJsonAsync("/tickets/999999", new { title = "Nobody" }, Token);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesTheTicket()
    {
        var client = factory.CreateClient();
        var created = await client.CreateTicketAsync("Duplicate ticket");

        var response = await client.DeleteAsync($"/tickets/{created["id"]}", Token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/tickets/{created["id"]}", Token)).StatusCode);
    }

    [Fact]
    public async Task List_IsPaged_WithTheConfiguredPageSize()
    {
        await using var small = TicketsApiFactory.With(("Tickets:PageSize", "2"));
        var client = small.CreateClient();
        for (var i = 0; i < 3; i++)
        {
            await client.CreateTicketAsync($"Paging {i}");
        }

        var page = await (await client.GetAsync("/tickets?page=1", Token)).ReadJsonAsync();

        Assert.Equal(2, page.AsArray().Count);
    }

    [Fact]
    public async Task OpenApiDocument_DescribesTheTicketEndpoints()
    {
        var response = await factory.CreateClient().GetAsync("/openapi/v1.json", Token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("/tickets/{id}", await response.Content.ReadAsStringAsync(Token));
    }
}
