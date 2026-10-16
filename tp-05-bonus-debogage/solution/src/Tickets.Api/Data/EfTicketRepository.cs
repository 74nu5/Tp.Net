using Microsoft.EntityFrameworkCore;
using Tickets.Api.Contracts;
using Tickets.Api.Domain;
using Tickets.Api.Import;
using Tickets.Api.Statistics;
using Tickets.Api.Telemetry;

namespace Tickets.Api.Data;

/// <summary>
/// TP-02, exercises 05 to 07: the EF Core implementation of <see cref="ITicketRepository"/>.
/// </summary>
public sealed class EfTicketRepository(TicketsDbContext db, TimeProvider clock, ILogger<EfTicketRepository> logger) : ITicketRepository
{
    // Read-only queries use AsNoTracking: EF does not keep a copy of each entity to detect changes,
    // which is faster and avoids surprises when the same context is reused.
    public async Task<IReadOnlyList<Ticket>> ListAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        await db.Tickets
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    // Include loads the comments in the same query (a JOIN). Without it, Comments stays empty:
    // EF Core does not lazy-load by default.
    public Task<Ticket?> GetAsync(int id, CancellationToken cancellationToken) =>
        db.Tickets
            .AsNoTracking()
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    // TP-02, exercise 06: a projection. EF translates the whole expression, Count included, into a single SQL query.
    // The starting version looped over the tickets and ran one COUNT query per ticket: the N+1 problem.
    public async Task<IReadOnlyList<TicketSummary>> GetSummariesAsync(CancellationToken cancellationToken) =>
        await db.Tickets
            .OrderBy(t => t.Id)
            .Select(t => new TicketSummary(t.Id, t.Title, t.Status, t.Comments.Count))
            .ToListAsync(cancellationToken);

    // TP-05, exercise 02: the starting version summed Ticket.CommentCount in the query. That property is plain C#,
    // not a column: EF Core cannot turn it into SQL and throws "The LINQ expression ... could not be translated"
    // when the query runs, not when it compiles. Counting the Comments table says the same thing in SQL.
    public async Task<TicketStats> GetStatsAsync(CancellationToken cancellationToken)
    {
        var tickets = await db.Tickets
            .Select(t => new TicketDates(t.Status, t.CreatedAt, t.ClosedAt))
            .ToListAsync(cancellationToken);
        var comments = await db.Comments.CountAsync(cancellationToken);
        return TicketStatistics.Compute(tickets, comments);
    }

    public async Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        ticket.CreatedAt = clock.GetUtcNow();
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync(cancellationToken);   // the INSERT happens here; Id is filled in afterwards
        return ticket;
    }

    // TP-05, exercise 04: a span of our own around the import. In the Aspire dashboard it groups the INSERT
    // commands under one named step of the trace, with the number of tickets as a tag.
    public async Task<int> ImportAsync(IReadOnlyList<ImportedTicket> tickets, CancellationToken cancellationToken)
    {
        using var activity = TicketsTelemetry.Source.StartActivity("ImportTickets");
        activity?.SetTag("tickets.count", tickets.Count);

        var now = clock.GetUtcNow();
        db.Tickets.AddRange(tickets.Select(t => new Ticket { Title = t.Title, Priority = t.Priority, Description = t.Description, CreatedAt = now }));
        await db.SaveChangesAsync(cancellationToken);   // EF Core batches the INSERT commands

        logger.TicketsImported(tickets.Count);
        return tickets.Count;
    }

    public async Task<bool> UpdateAsync(int id, string title, string? description, TicketPriority priority, CancellationToken cancellationToken)
    {
        // Tracked query: EF remembers the original values and SaveChanges sends only what changed.
        // TP-05, exercise 06 (anomaly 1): the starting version added AsNoTracking() here. The entity was then unknown
        // to the context, SaveChanges found nothing to save and sent no UPDATE, and the endpoint still answered 204.
        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (ticket is null)
        {
            return false;
        }

        ticket.Title = title;
        ticket.Description = description;
        ticket.Priority = priority;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    // ExecuteDeleteAsync sends a DELETE directly, without loading the entity first.
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        await db.Tickets.Where(t => t.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;

    public async Task<(TicketChange Result, Comment? Comment)> AddCommentAsync(int ticketId, string author, string text, CancellationToken cancellationToken)
    {
        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken);
        if (ticket is null)
        {
            return (TicketChange.NotFound, null);
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return (TicketChange.AlreadyClosed, null);
        }

        var comment = new Comment { Author = author, Text = text, CreatedAt = clock.GetUtcNow() };
        ticket.Comments.Add(comment);   // change tracking: adding to a tracked collection is enough
        await db.SaveChangesAsync(cancellationToken);
        return (TicketChange.Applied, comment);
    }

    public async Task<(TicketChange Result, Ticket? Ticket)> CloseAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await db.Tickets.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (ticket is null)
        {
            return (TicketChange.NotFound, null);
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return (TicketChange.AlreadyClosed, ticket);
        }

        ticket.Status = TicketStatus.Closed;
        ticket.ClosedAt = clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);   // a single UPDATE with the two changed columns

        // TP-05, exercise 03: a structured log entry, see TicketLog.
        logger.TicketClosed(ticket.Id, Math.Round((ticket.ClosedAt.Value - ticket.CreatedAt).TotalHours, 1));
        return (TicketChange.Applied, ticket);
    }
}
