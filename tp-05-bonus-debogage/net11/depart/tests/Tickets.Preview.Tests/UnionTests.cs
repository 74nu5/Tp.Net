using Microsoft.Extensions.Time.Testing;

namespace Tickets.Preview.Tests.Exercise05;

public class UnionTests
{
    private static readonly DateTimeOffset Friday = new(2026, 10, 23, 11, 0, 0, TimeSpan.Zero);

    private readonly TicketBoard _board = new(new FakeTimeProvider(Friday));

    [Fact]
    public void ClosingAnOpenTicket_GivesClosed()
    {
        var ticket = _board.Add("Printer jam");

        var result = _board.Close(ticket.Id);

        // Value holds the case: a Closed here, typed as object.
        var closed = Assert.IsType<Closed>(result.Value);
        Assert.Equal(Friday, closed.Ticket.ClosedAt);
        Assert.Equal(200, result.ToStatusCode());
        Assert.Equal($"Ticket {ticket.Id} closed.", result.Describe());
    }

    [Fact]
    public void ClosingAnUnknownTicket_GivesNotFound()
    {
        var result = _board.Close(42);

        Assert.Equal(new NotFound(42), result.Value);
        Assert.Equal(404, result.ToStatusCode());
        Assert.Equal("No ticket has the id 42.", result.Describe());
    }

    [Fact]
    public void ClosingTwice_GivesAlreadyClosed()
    {
        var ticket = _board.Add("Printer jam");
        _board.Close(ticket.Id);

        var result = _board.Close(ticket.Id);

        Assert.IsType<AlreadyClosed>(result.Value);
        Assert.Equal(409, result.ToStatusCode());
        Assert.Equal($"Ticket {ticket.Id} was already closed on 2026-10-23.", result.Describe());
    }
}
