using Tickets.Api.Contracts;
using Tickets.Api.Domain;
using Tickets.Api.Import;
using Tickets.Api.Statistics;

namespace Tickets.Api.Data;

/// <summary>Outcome of a change that can be refused.</summary>
public enum TicketChange
{
    Applied,
    NotFound,
    AlreadyClosed,
}

/// <summary>
/// Everything the endpoints need from storage. The endpoints depend on this interface,
/// so the storage can change (in memory, then EF Core) without touching them: that is what DI is for.
/// </summary>
public interface ITicketRepository
{
    Task<IReadOnlyList<Ticket>> ListAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>The ticket with its comments, or null.</summary>
    Task<Ticket?> GetAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<TicketSummary>> GetSummariesAsync(CancellationToken cancellationToken);

    /// <summary>TP-05: counts and average resolution time.</summary>
    Task<TicketStats> GetStatsAsync(CancellationToken cancellationToken);

    Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken);

    /// <summary>TP-05: creates the tickets read from a CSV export; returns how many were created.</summary>
    Task<int> ImportAsync(IReadOnlyList<ImportedTicket> tickets, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(int id, string title, string? description, TicketPriority priority, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<(TicketChange Result, Comment? Comment)> AddCommentAsync(int ticketId, string author, string text, CancellationToken cancellationToken);

    Task<(TicketChange Result, Ticket? Ticket)> CloseAsync(int id, CancellationToken cancellationToken);
}
