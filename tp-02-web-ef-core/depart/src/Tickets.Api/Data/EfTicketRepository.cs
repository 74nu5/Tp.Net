using Microsoft.EntityFrameworkCore;
using Tickets.Api.Contracts;
using Tickets.Api.Domain;

namespace Tickets.Api.Data;

/// <summary>
/// Exercises 05 to 07: the EF Core implementation of <see cref="ITicketRepository"/>.
/// </summary>
public sealed class EfTicketRepository(TicketsDbContext db, TimeProvider clock) : ITicketRepository
{
    // The current time, through an abstraction that tests can replace. To use in CreateAsync and CloseAsync.
    private readonly TimeProvider _clock = clock;

    // TODO Exercise 05: the tickets of the page, ordered by id.
    public Task<IReadOnlyList<Ticket>> ListAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO Exercise 05");

    // TODO Exercise 05: the ticket, or null.
    public Task<Ticket?> GetAsync(int id, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO Exercise 05");

    // Provided, and slow. TODO Exercise 06: find out why, then fix it.
    public async Task<IReadOnlyList<TicketSummary>> GetSummariesAsync(CancellationToken cancellationToken)
    {
        var tickets = await db.Tickets.OrderBy(t => t.Id).ToListAsync(cancellationToken);
        var summaries = new List<TicketSummary>();
        foreach (var ticket in tickets)
        {
            var commentCount = await db.Comments.CountAsync(c => c.TicketId == ticket.Id, cancellationToken);
            summaries.Add(new TicketSummary(ticket.Id, ticket.Title, ticket.Status, commentCount));
        }

        return summaries;
    }

    // TODO Exercise 05: set CreatedAt with _clock.GetUtcNow(), add the ticket and save.
    public Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO Exercise 05");

    // TODO Exercise 05: false when the ticket does not exist.
    public Task<bool> UpdateAsync(int id, string title, string? description, TicketPriority priority, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO Exercise 05");

    // TODO Exercise 05: false when the ticket does not exist.
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO Exercise 05");

    // TODO Exercise 07: NotFound, AlreadyClosed when the ticket is closed, otherwise add the comment and save.
    public Task<(TicketChange Result, Comment? Comment)> AddCommentAsync(int ticketId, string author, string text, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO Exercise 07");

    // TODO Exercise 07: NotFound, AlreadyClosed, otherwise set Status and ClosedAt and save.
    public Task<(TicketChange Result, Ticket? Ticket)> CloseAsync(int id, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO Exercise 07");
}
