using Tickets.Api.Domain;

namespace Tickets.Api.Import;

/// <summary>A ticket read from the export of the old ticketing tool.</summary>
public sealed record ImportedTicket(string Title, TicketPriority Priority, string? Description);

/// <summary>
/// TP-05, exercise 01: reads the CSV export of the old tool: a header line, then <c>title,priority,description</c>.
/// </summary>
public static class TicketCsvParser
{
    private const int FieldCount = 3;

    public static IReadOnlyList<ImportedTicket> Parse(TextReader reader)
    {
        var tickets = new List<ImportedTicket>();
        var lineNumber = 0;

        while (reader.ReadLine() is { } line)
        {
            lineNumber++;
            if (lineNumber == 1 || line.Length == 0)
            {
                continue;   // the header, and blank lines
            }

            var fields = line.Split(',');
            if (fields.Length == FieldCount)
            {
                var priority = Enum.TryParse<TicketPriority>(fields[1], ignoreCase: true, out var parsed) ? parsed : TicketPriority.Normal;
                var description = fields[2].Length == 0 ? null : fields[2];
                tickets.Add(new ImportedTicket(fields[0], priority, description));
            }
        }

        return tickets;
    }
}
