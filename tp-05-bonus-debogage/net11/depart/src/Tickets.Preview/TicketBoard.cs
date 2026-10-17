namespace Tickets.Preview;

public enum TicketStatus
{
    Open,
    Closed,
}

public sealed record Ticket(int Id, string Title, TicketStatus Status = TicketStatus.Open, DateTimeOffset? ClosedAt = null);

// The three possible outcomes of closing a ticket. In the Tickets API (TP-02 to TP-05), the same idea is a tuple
// (TicketChange Result, Ticket? Ticket): nothing stops a caller from reading Ticket when the result is NotFound.
public sealed record Closed(Ticket Ticket);

public sealed record NotFound(int Id);

public sealed record AlreadyClosed(Ticket Ticket);

/// <summary>
/// C# 15: a union type. A CloseResult holds exactly one of the case types, and each case converts implicitly
/// to it. A switch on a CloseResult must handle every case, or the compiler reports CS8509 (an error in this project).
/// </summary>
// TODO Exercise 05: AlreadyClosed is not a case of the union yet. Add it, then let the compiler show you
// every switch that must handle it.
public union CloseResult(Closed, NotFound);

/// <summary>A tiny in-memory ticket store, to try the union without a database.</summary>
public sealed class TicketBoard(TimeProvider clock)
{
    private readonly Dictionary<int, Ticket> _tickets = [];

    public Ticket Add(string title)
    {
        var ticket = new Ticket(_tickets.Count + 1, title);
        _tickets.Add(ticket.Id, ticket);
        return ticket;
    }

    // Each return converts a case type to CloseResult implicitly: no wrapper, no common base class.
    public CloseResult Close(int id)
    {
        if (!_tickets.TryGetValue(id, out var ticket))
        {
            return new NotFound(id);
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            // TODO Exercise 05: return an AlreadyClosed instead of throwing.
            throw new InvalidOperationException($"Ticket {id} is already closed.");
        }

        var closed = ticket with { Status = TicketStatus.Closed, ClosedAt = clock.GetUtcNow() };
        _tickets[id] = closed;
        return new Closed(closed);
    }
}

/// <summary>What the API would answer for each outcome.</summary>
public static class CloseResultExtensions
{
    // The switch is exhaustive without a discard arm: add a case to the union and this stops building.
    public static int ToStatusCode(this CloseResult result) => result switch
    {
        Closed => 200,
        NotFound => 404,
    };

    // Patterns go inside the cases: a positional pattern deconstructs the record.
    public static string Describe(this CloseResult result) => result switch
    {
        Closed(var ticket) => $"Ticket {ticket.Id} closed.",
        NotFound(var id) => $"No ticket has the id {id}.",
    };
}
