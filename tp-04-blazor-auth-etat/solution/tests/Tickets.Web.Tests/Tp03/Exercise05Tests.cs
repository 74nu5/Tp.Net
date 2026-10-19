using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Web.Client.Api;
using Tickets.Web.Components.Pages;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Tp03.Exercise05;

public class NewTicketFormTests : ComponentTest
{
    [Fact]
    public void ValidForm_CreatesTheTicket_AndGoesBackToTheList()
    {
        var page = Render<NewTicket>();

        page.Find("#title").Change("Printer on fire");
        page.Find("#description").Change("Third floor");
        page.Find("#priority").Change("High");
        page.Find("form").Submit();

        page.WaitForAssertion(() => Assert.Single(Api.Created));
        var created = Api.Created[0];
        Assert.Equal("Printer on fire", created.Title);
        Assert.Equal("Third floor", created.Description);
        Assert.Equal(TicketPriority.High, created.Priority);
        Assert.EndsWith("/tickets", Services.GetRequiredService<NavigationManager>().Uri);
    }

    [Fact]
    public void EmptyTitle_ShowsAValidationMessage_AndSendsNothing()
    {
        var page = Render<NewTicket>();

        page.Find("form").Submit();

        Assert.NotEmpty(page.FindAll(".validation-message"));
        Assert.Empty(Api.Created);
    }

    [Fact]
    public void TooLongTitle_IsRefused()
    {
        var page = Render<NewTicket>();

        page.Find("#title").Change(new string('x', 201));
        page.Find("form").Submit();

        Assert.NotEmpty(page.FindAll(".validation-message"));
        Assert.Empty(Api.Created);
    }
}
