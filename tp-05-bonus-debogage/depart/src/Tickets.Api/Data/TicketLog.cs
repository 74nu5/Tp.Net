namespace Tickets.Api.Data;

/// <summary>
/// TP-05, exercise 03: the log messages of the repository, generated at compile time by [LoggerMessage].
/// </summary>
/// <remarks>
/// The message is a template, not an interpolated string: TicketId and OpenHours become fields of the log entry,
/// that the Aspire dashboard (or Seq, Elastic...) can filter on. The generated code also skips all the work,
/// boxing included, when the level is disabled.
/// </remarks>
internal static partial class TicketLog
{
    // TODO Exercise 03: a TicketClosed message, "Ticket {TicketId} closed after {OpenHours} hours", event id 1001.

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "Imported {Count} tickets")]
    public static partial void TicketsImported(this ILogger logger, int count);
}
