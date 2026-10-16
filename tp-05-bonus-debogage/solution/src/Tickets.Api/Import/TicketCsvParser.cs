using System.Text;
using Tickets.Api.Domain;

namespace Tickets.Api.Import;

/// <summary>A ticket read from the export of the old ticketing tool.</summary>
public sealed record ImportedTicket(string Title, TicketPriority Priority, string? Description);

/// <summary>
/// TP-05, exercise 01: reads the CSV export of the old tool: a header line, then <c>title,priority,description</c>.
/// </summary>
/// <remarks>
/// Fields that contain a comma are quoted, and a quote inside a quoted field is doubled (RFC 4180):
/// <c>"Printer, 2nd floor",High,"Says ""paper jam"""</c>. Line breaks inside a field are not supported.
/// The starting version split each line on every comma and silently skipped the lines that did not give
/// three fields: the quoted ones. A line that is still malformed now stops the import with its line number,
/// rather than losing data without a word.
/// </remarks>
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

            var fields = SplitFields(line);
            if (fields.Count != FieldCount)
            {
                throw new FormatException($"Line {lineNumber}: expected {FieldCount} fields, found {fields.Count}.");
            }

            var priority = Enum.TryParse<TicketPriority>(fields[1], ignoreCase: true, out var parsed) ? parsed : TicketPriority.Normal;
            var description = fields[2].Length == 0 ? null : fields[2];
            tickets.Add(new ImportedTicket(fields[0], priority, description));
        }

        return tickets;
    }

    /// <summary>Splits one CSV line, honouring quoted fields.</summary>
    public static IReadOnlyList<string> SplitFields(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (inQuotes)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');   // "" inside quotes is one quote
                    i++;
                }
                else if (c == '"')
                {
                    inQuotes = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == ',')
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
