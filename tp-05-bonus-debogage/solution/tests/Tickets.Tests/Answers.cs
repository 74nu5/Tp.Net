namespace Tickets.Tests;

/// <summary>
/// What you found with the tools of exercises 01 and 02. The tests compare these values with the expected ones.
/// </summary>
public static class Answers
{
    /// <summary>
    /// Exercise 01: the number of the first line of samples/legacy-tickets.csv that the parser skips.
    /// The header is line 1. Found with a conditional breakpoint in TicketCsvParser.Parse.
    /// </summary>
    public const int FirstSkippedLine = 87;

    /// <summary>
    /// Exercise 02: the first method of our own code in the stack trace of GET /tickets/stats,
    /// written as Class.Method, e.g. "TicketEndpoints.ListAsync".
    /// </summary>
    public const string StatsFailingMethod = "EfTicketRepository.GetStatsAsync";
}
