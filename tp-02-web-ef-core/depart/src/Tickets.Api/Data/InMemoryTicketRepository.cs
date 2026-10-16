using System.Collections.Concurrent;
using Tickets.Api.Contracts;
using Tickets.Api.Domain;

namespace Tickets.Api.Data;

/// <summary>
/// Provided. A repository that keeps the tickets in memory, to write the endpoints of exercise 04
/// before having a database. Everything is lost when the application stops.
/// </summary>
/// <remarks>Registered as a singleton: one shared instance, hence the thread-safe dictionary.</remarks>
public sealed class InMemoryTicketRepository : ITicketRepository
{
    private readonly ConcurrentDictionary<int, Ticket> _tickets = new();
    private int _lastTicketId;
    private int _lastCommentId;

    public Task<IReadOnlyList<Ticket>> ListAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Ticket>>([.. _tickets.Values.OrderBy(t => t.Id).Skip((page - 1) * pageSize).Take(pageSize)]);

    public Task<Ticket?> GetAsync(int id, CancellationToken cancellationToken) =>
        Task.FromResult(_tickets.GetValueOrDefault(id));

    public Task<IReadOnlyList<TicketSummary>> GetSummariesAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<TicketSummary>>(
            [.. _tickets.Values.OrderBy(t => t.Id).Select(t => new TicketSummary(t.Id, t.Title, t.Status, t.Comments.Count))]);

    public Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        ticket.Id = Interlocked.Increment(ref _lastTicketId);
        ticket.CreatedAt = DateTimeOffset.UtcNow;
        _tickets[ticket.Id] = ticket;
        return Task.FromResult(ticket);
    }

    public Task<bool> UpdateAsync(int id, string title, string? description, TicketPriority priority, CancellationToken cancellationToken)
    {
        if (!_tickets.TryGetValue(id, out var ticket))
        {
            return Task.FromResult(false);
        }

        ticket.Title = title;
        ticket.Description = description;
        ticket.Priority = priority;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        Task.FromResult(_tickets.TryRemove(id, out _));

    public Task<(TicketChange Result, Comment? Comment)> AddCommentAsync(int ticketId, string author, string text, CancellationToken cancellationToken)
    {
        if (!_tickets.TryGetValue(ticketId, out var ticket))
        {
            return Task.FromResult<(TicketChange, Comment?)>((TicketChange.NotFound, null));
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return Task.FromResult<(TicketChange, Comment?)>((TicketChange.AlreadyClosed, null));
        }

        var comment = new Comment { Id = Interlocked.Increment(ref _lastCommentId), TicketId = ticketId, Author = author, Text = text, CreatedAt = DateTimeOffset.UtcNow };
        ticket.Comments.Add(comment);
        return Task.FromResult<(TicketChange, Comment?)>((TicketChange.Applied, comment));
    }

    public Task<(TicketChange Result, Ticket? Ticket)> CloseAsync(int id, CancellationToken cancellationToken)
    {
        if (!_tickets.TryGetValue(id, out var ticket))
        {
            return Task.FromResult<(TicketChange, Ticket?)>((TicketChange.NotFound, null));
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return Task.FromResult<(TicketChange, Ticket?)>((TicketChange.AlreadyClosed, ticket));
        }

        ticket.Status = TicketStatus.Closed;
        return Task.FromResult<(TicketChange, Ticket?)>((TicketChange.Applied, ticket));
    }
}
