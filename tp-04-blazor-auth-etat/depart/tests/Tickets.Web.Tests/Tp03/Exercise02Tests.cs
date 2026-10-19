using Microsoft.AspNetCore.Components;
using Tickets.Web.Client.Api;
using Tickets.Web.Client.Components;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Tp03.Exercise02;

// The parameters are passed by name (Named), so that these tests compile before the components declare them.
public class PriorityBadgeTests : ComponentTest
{
    [Theory]
    [InlineData(TicketPriority.Low, "badge-low")]
    [InlineData(TicketPriority.High, "badge-high")]
    public void Badge_ShowsThePriority_WithItsCssClass(TicketPriority priority, string cssClass)
    {
        var badge = Render<PriorityBadge>(parameters => parameters.Named("Priority", priority));

        var span = badge.Find("span.badge");
        Assert.Equal(priority.ToString(), span.TextContent.Trim());
        Assert.Contains(cssClass, span.ClassList);
    }
}

public class TicketCardTests : ComponentTest
{
    private static readonly TicketDto Printer = FakeTicketsClient.Ticket(7, "Printer on fire", TicketPriority.High, "Third floor");

    [Fact]
    public void Card_ShowsTheTicket_AndItsPriorityBadge()
    {
        var card = Render<TicketCard>(parameters => parameters.Named("Ticket", Printer));

        Assert.Equal("Printer on fire", card.Find(".ticket-card h3").TextContent.Trim());
        Assert.Contains("Third floor", card.Markup);
        Assert.Contains("badge-high", card.Find("span.badge").ClassList);
    }

    [Fact]
    public void ClickingSelect_TellsTheParentWhichTicket()
    {
        TicketDto? selected = null;
        var card = Render<TicketCard>(parameters => parameters
            .Named("Ticket", Printer)
            .Named("OnSelected", EventCallback.Factory.Create<TicketDto>(this, ticket => selected = ticket)));

        card.Find("button.select").Click();

        Assert.Equal(Printer, selected);
    }

    [Fact]
    public void Card_RendersTheContentGivenByTheParent()
    {
        RenderFragment extra = builder => builder.AddMarkupContent(0, "<em class=\"extra\">Assigned to Alice</em>");

        var card = Render<TicketCard>(parameters => parameters
            .Named("Ticket", Printer)
            .Named("ChildContent", extra));

        Assert.Equal("Assigned to Alice", card.Find(".ticket-card em.extra").TextContent);
    }
}
