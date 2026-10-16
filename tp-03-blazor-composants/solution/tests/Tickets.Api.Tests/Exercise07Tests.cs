using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Api.Data;
using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise07;

[Collection(DatabaseCollection.Name)]
public class WriteTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private CancellationToken Token => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Comment_IsAddedToTheTicket()
    {
        var client = factory.CreateClient();
        var id = (await client.CreateTicketAsync(HttpExtensions.UniqueTitle("Comment me")))["id"];

        var response = await client.PostAsJsonAsync($"/tickets/{id}/comments", new { author = "Alice", text = "I can reproduce it." }, Token);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var ticket = await (await client.GetAsync($"/tickets/{id}", Token)).ReadJsonAsync();
        var comment = Assert.Single(ticket["comments"]!.AsArray())!;
        Assert.Equal("Alice", comment["author"]!.GetValue<string>());
    }

    [Fact]
    public async Task Comment_OnAnUnknownTicket_IsA404()
    {
        var response = await factory.CreateClient().PostAsJsonAsync("/tickets/999999/comments", new { author = "Alice", text = "Hello?" }, Token);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Close_SetsTheStatusAndTheClosingDate()
    {
        var client = factory.CreateClient();
        var id = (await client.CreateTicketAsync(HttpExtensions.UniqueTitle("Close me")))["id"];

        var response = await client.PostAsync($"/tickets/{id}/close", null, Token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var closed = await response.ReadJsonAsync();
        Assert.Equal("Closed", closed["status"]!.GetValue<string>());
        Assert.NotNull(closed["closedAt"]);
    }

    [Fact]
    public async Task Closing_Twice_IsAConflict()
    {
        var client = factory.CreateClient();
        var id = (await client.CreateTicketAsync(HttpExtensions.UniqueTitle("Close twice")))["id"];
        await client.PostAsync($"/tickets/{id}/close", null, Token);

        var response = await client.PostAsync($"/tickets/{id}/close", null, Token);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Comment_OnAClosedTicket_IsAConflict()
    {
        var client = factory.CreateClient();
        var id = (await client.CreateTicketAsync(HttpExtensions.UniqueTitle("Closed then commented")))["id"];
        await client.PostAsync($"/tickets/{id}/close", null, Token);

        var response = await client.PostAsJsonAsync($"/tickets/{id}/comments", new { author = "Bob", text = "Too late" }, Token);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task ClosingDate_IsAddedByASecondMigration()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var migrations = scope.ServiceProvider.GetRequiredService<TicketsDbContext>().Database.GetMigrations().ToList();

        Assert.True(migrations.Count >= 2, "Expected two migrations: InitialCreate, then AddClosedAt.");
        Assert.EndsWith("_AddClosedAt", migrations[^1]);
    }
}
