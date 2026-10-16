using Microsoft.Extensions.Caching.Memory;
using Tickets.Api.Contracts;
using Tickets.Api.Domain;
using Tickets.Api.Import;
using Tickets.Api.Statistics;

namespace Tickets.Api.Data;

/// <summary>
/// A decorator: implements <see cref="ITicketRepository"/> by delegating to another implementation,
/// and keeps the summaries in memory for a minute. Every write empties that cache.
/// </summary>
public sealed class CachedTicketRepository(ITicketRepository inner, IMemoryCache cache) : ITicketRepository
{
    public async Task<IReadOnlyList<TicketSummary>> GetSummariesAsync(CancellationToken cancellationToken) =>
        await cache.GetOrCreateAsync("tickets:summaries", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return inner.GetSummariesAsync(cancellationToken);
        }) ?? [];

    public Task<IReadOnlyList<Ticket>> ListAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        inner.ListAsync(page, pageSize, cancellationToken);

    public Task<Ticket?> GetAsync(int id, CancellationToken cancellationToken) => inner.GetAsync(id, cancellationToken);

    public Task<TicketStats> GetStatsAsync(CancellationToken cancellationToken) => inner.GetStatsAsync(cancellationToken);

    public async Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        var created = await inner.CreateAsync(ticket, cancellationToken);
        Invalidate();
        return created;
    }

    public async Task<int> ImportAsync(IReadOnlyList<ImportedTicket> tickets, CancellationToken cancellationToken)
    {
        var count = await inner.ImportAsync(tickets, cancellationToken);
        Invalidate();
        return count;
    }

    public async Task<bool> UpdateAsync(int id, string title, string? description, TicketPriority priority, CancellationToken cancellationToken)
    {
        var updated = await inner.UpdateAsync(id, title, description, priority, cancellationToken);
        Invalidate();
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await inner.DeleteAsync(id, cancellationToken);
        Invalidate();
        return deleted;
    }

    public async Task<(TicketChange Result, Comment? Comment)> AddCommentAsync(int ticketId, string author, string text, CancellationToken cancellationToken)
    {
        var result = await inner.AddCommentAsync(ticketId, author, text, cancellationToken);
        Invalidate();
        return result;
    }

    public async Task<(TicketChange Result, Ticket? Ticket)> CloseAsync(int id, CancellationToken cancellationToken)
    {
        var result = await inner.CloseAsync(id, cancellationToken);
        Invalidate();
        return result;
    }

    private void Invalidate() => cache.Remove("tickets:summary");
}
