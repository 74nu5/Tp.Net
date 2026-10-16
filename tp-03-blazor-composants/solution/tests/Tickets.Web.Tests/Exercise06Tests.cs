using Tickets.Web.Client.Api;
using Tickets.Web.Components.Pages;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Exercise06;

public class TicketGridTests : ComponentTest
{
    public TicketGridTests()
    {
        string[] titles = ["Mouse", "Printer", "Keyboard", "Screen", "Laptop", "Cable", "Desk"];
        for (var i = 0; i < titles.Length; i++)
        {
            Api.Tickets.Add(FakeTicketsClient.Ticket(i + 1, titles[i], i % 3 == 0 ? TicketPriority.High : TicketPriority.Normal));
        }
    }

    private static IEnumerable<string> Titles(IRenderedComponent<TicketGrid> grid) =>
        grid.FindAll("tbody tr").Select(row => row.Children[1].TextContent.Trim());

    [Fact]
    public void Grid_ShowsFiveTicketsPerPage()
    {
        var grid = Render<TicketGrid>();

        grid.WaitForAssertion(() => Assert.Equal(5, grid.FindAll("tbody tr").Count));
    }

    [Fact]
    public void Grid_IsSortedByTitle_ByDefault()
    {
        var grid = Render<TicketGrid>();

        grid.WaitForAssertion(() => Assert.Equal(["Cable", "Desk", "Keyboard", "Laptop", "Mouse"], Titles(grid)));
    }

    [Fact]
    public void ClickingTheTitleHeader_ReversesTheOrder()
    {
        var grid = Render<TicketGrid>();
        grid.WaitForAssertion(() => Assert.Equal(5, grid.FindAll("tbody tr").Count));

        grid.FindAll("thead button.col-title").Single(button => button.TextContent.Contains("Title")).Click();

        grid.WaitForAssertion(() => Assert.Equal("Screen", Titles(grid).First()));
    }

    [Fact]
    public void Grid_ShowsThePriorityBadge()
    {
        var grid = Render<TicketGrid>();

        grid.WaitForAssertion(() => Assert.NotEmpty(grid.FindAll("tbody span.badge")));
    }
}
