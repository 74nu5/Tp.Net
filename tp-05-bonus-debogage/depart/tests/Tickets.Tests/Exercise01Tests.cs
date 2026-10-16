using Tickets.Api.Domain;
using Tickets.Api.Import;
using Tickets.Tests.Support;

namespace Tickets.Tests.Exercise01;

public class CsvImportTests
{
    private static IReadOnlyList<ImportedTicket> ParseSample()
    {
        using var reader = File.OpenText(Path.Combine(AppContext.BaseDirectory, "legacy-tickets.csv"));
        return TicketCsvParser.Parse(reader);
    }

    [Fact]
    public void TheFirstSkippedLine_IsTheOneTheDebuggerShowed() =>
        Assert.Equal("bdd2d3af3a5a1213", AnswerCheck.Hash(Answers.FirstSkippedLine.ToString(System.Globalization.CultureInfo.InvariantCulture)));

    [Fact]
    public void TheSample_GivesOneTicketPerLine() =>
        Assert.Equal(240, ParseSample().Count);

    [Fact]
    public void AQuotedField_KeepsItsComma() =>
        Assert.Contains(ParseSample(), ticket => ticket is { Title: "Printer, 2nd floor", Priority: TicketPriority.High });

    [Fact]
    public void QuotesAroundAField_AreNotPartOfIt() =>
        Assert.Contains(ParseSample(), ticket => ticket.Title == "Login page slow on Mondays");

    [Fact]
    public void ADoubledQuote_IsOneQuote() =>
        Assert.Contains(ParseSample(), ticket =>
            ticket.Title == "Export fails with \"Access denied\", even for admins"
            && ticket.Description == "Seen on the CSV, XLSX and PDF exports");

    [Fact]
    public void AMalformedLine_StopsTheImportWithItsNumber()
    {
        var exception = Assert.Throws<FormatException>(() =>
            TicketCsvParser.Parse(new StringReader("title,priority,description\nPrinter jam,High,Second floor\nNo priority at all\n")));
        Assert.Contains("Line 3", exception.Message);
    }

    [Fact]
    public void ThePriority_IgnoresCase_AndDefaultsToNormal()
    {
        var tickets = TicketCsvParser.Parse(new StringReader("title,priority,description\nA,HIGH,\nB,Urgent,\n"));
        Assert.Equal([TicketPriority.High, TicketPriority.Normal], tickets.Select(ticket => ticket.Priority));
    }
}
