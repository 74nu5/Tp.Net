using Tickets.Web.Components.Pages;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Tp03.Exercise03;

public class TicketListTests : ComponentTest
{
    [Fact]
    public void WhileLoading_ShowsALoadingMessage()
    {
        Api.Gate = new TaskCompletionSource();

        var page = Render<TicketList>();

        Assert.NotNull(page.Find(".loading"));
    }

    [Fact]
    public void OnceLoaded_ShowsOneCardPerTicket()
    {
        Api.Tickets.AddRange([FakeTicketsClient.Ticket(1, "Printer"), FakeTicketsClient.Ticket(2, "Screen"), FakeTicketsClient.Ticket(3, "Mouse")]);

        var page = Render<TicketList>();

        page.WaitForAssertion(() => Assert.Equal(3, page.FindAll(".ticket-card").Count));
    }

    [Fact]
    public void WithoutTickets_ShowsAnEmptyState()
    {
        var page = Render<TicketList>();

        page.WaitForAssertion(() => Assert.NotNull(page.Find(".empty")));
    }

    [Fact]
    public void WhenTheApiFails_ShowsAnError()
    {
        Api.Failure = new HttpRequestException("Connection refused");

        var page = Render<TicketList>();

        page.WaitForAssertion(() => Assert.Contains("Connection refused", page.Find(".error").TextContent));
    }

    [Fact]
    public void SelectingACard_ShowsTheSelectedTicket()
    {
        Api.Tickets.AddRange([FakeTicketsClient.Ticket(1, "Printer"), FakeTicketsClient.Ticket(2, "Screen")]);
        var page = Render<TicketList>();
        page.WaitForAssertion(() => Assert.Equal(2, page.FindAll(".ticket-card").Count));

        page.FindAll("button.select")[1].Click();

        Assert.Contains("Screen", page.Find(".selected").TextContent);
    }
}
