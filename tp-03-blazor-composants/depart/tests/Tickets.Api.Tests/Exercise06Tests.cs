using Microsoft.Extensions.DependencyInjection;
using Tickets.Api.Data;
using Tickets.Api.Domain;
using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise06;

[Collection(DatabaseCollection.Name)]
public class QueryTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private CancellationToken Token => TestContext.Current.CancellationToken;

    /// <summary>Inserts a ticket with comments straight into the database, without going through the API.</summary>
    private async Task<int> SeedTicketAsync(string title, int comments)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TicketsDbContext>();
        var ticket = new Ticket { Title = title, CreatedAt = DateTimeOffset.UtcNow };
        for (var i = 1; i <= comments; i++)
        {
            ticket.Comments.Add(new Comment { Author = "seed", Text = $"Comment {i}", CreatedAt = DateTimeOffset.UtcNow });
        }

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync(Token);
        return ticket.Id;
    }

    [Fact]
    public async Task GetById_ReturnsTheComments()
    {
        var id = await SeedTicketAsync(HttpExtensions.UniqueTitle("With comments"), comments: 2);

        var json = await (await factory.CreateClient().GetAsync($"/tickets/{id}", Token)).ReadJsonAsync();

        Assert.Equal(2, json["comments"]!.AsArray().Count);
    }

    [Fact]
    public async Task Summary_CountsTheComments()
    {
        var title = HttpExtensions.UniqueTitle("Summary");
        var id = await SeedTicketAsync(title, comments: 3);

        var summary = await (await factory.CreateClient().GetAsync("/tickets/summary", Token)).ReadJsonAsync();

        var line = summary.AsArray().Single(item => item!["id"]!.GetValue<int>() == id)!;
        Assert.Equal(title, line["title"]!.GetValue<string>());
        Assert.Equal(3, line["commentCount"]!.GetValue<int>());
    }

    [Fact]
    public async Task Summary_RunsASingleSqlQuery_WhateverTheNumberOfTickets()
    {
        for (var i = 0; i < 3; i++)
        {
            await SeedTicketAsync(HttpExtensions.UniqueTitle("N+1"), comments: 1);
        }

        var client = factory.CreateClient();
        factory.SqlCommands.Reset();
        await client.GetAsync("/tickets/summary", Token);

        Assert.True(factory.SqlCommands.Count == 1, $"GET /tickets/summary sent {factory.SqlCommands.Count} SQL commands; one is enough.");
    }

    [Fact]
    public async Task Reads_DoNotTrackTheEntities()
    {
        var id = await SeedTicketAsync(HttpExtensions.UniqueTitle("Tracking"), comments: 1);
        await using var scope = factory.Services.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        var db = scope.ServiceProvider.GetRequiredService<TicketsDbContext>();

        await repository.ListAsync(1, 10, Token);
        await repository.GetAsync(id, Token);

        Assert.Empty(db.ChangeTracker.Entries());
    }
}
