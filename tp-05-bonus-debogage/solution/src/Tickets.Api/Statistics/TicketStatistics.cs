using Tickets.Api.Domain;

namespace Tickets.Api.Statistics;

/// <summary>What GET /tickets/stats returns.</summary>
public sealed record TicketStats(int Open, int Closed, int Comments, double? AverageResolutionHours);

/// <summary>The dates the statistics need, projected by the query rather than whole entities.</summary>
public sealed record TicketDates(TicketStatus Status, DateTimeOffset CreatedAt, DateTimeOffset? ClosedAt);

/// <summary>TP-05, exercise 06 (anomaly 3): computes the statistics from the tickets' dates.</summary>
public static class TicketStatistics
{
    public static TicketStats Compute(IReadOnlyCollection<TicketDates> tickets, int comments)
    {
        var closed = tickets.Where(t => t.Status == TicketStatus.Closed && t.ClosedAt is not null).ToList();

        // Average throws "Sequence contains no elements" on an empty sequence: a new database has no closed
        // ticket yet. "No value" is the honest answer, hence null rather than 0.
        double? averageHours = closed.Count == 0
            ? null
            : Math.Round(closed.Average(t => (t.ClosedAt!.Value - t.CreatedAt).TotalHours), 1);

        return new TicketStats(tickets.Count - closed.Count, closed.Count, comments, averageHours);
    }
}
