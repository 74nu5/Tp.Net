using System.Text.RegularExpressions;
using Tickets.Web.Client.Components;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Exercise07;

public class JavaScriptInteropTests : ComponentTest
{
    [Fact]
    public void CopyButton_CallsTheJavaScriptModule_WithTheTitle()
    {
        var module = JSInterop.SetupModule("./Components/TicketCard.razor.js");
        module.SetupVoid("copyText", "Printer on fire").SetVoidResult();
        var card = Render<TicketCard>(parameters => parameters.Named("Ticket", FakeTicketsClient.Ticket(1, "Printer on fire")));

        card.Find("button.copy").Click();

        module.VerifyInvoke("copyText");
    }
}

public partial class ScopedCssTests(WebAppFactory app) : IClassFixture<WebAppFactory>
{
    [Fact]
    public async Task TicketCard_HasItsOwnScopedStyles()
    {
        // Tickets.Web.styles.css imports the bundle of the client project, which holds TicketCard.razor.css.
        var bundle = await app.GetHtmlAsync("/Tickets.Web.styles.css");
        var clientBundle = ClientBundle().Match(bundle);
        Assert.True(clientBundle.Success, "No scoped CSS bundle for Tickets.Web.Client: add TicketCard.razor.css next to TicketCard.razor.");

        var css = await app.GetHtmlAsync("/" + clientBundle.Groups[1].Value);

        Assert.Matches(@"\.ticket-card\[b-[a-z0-9]+\]", css);
    }

    [GeneratedRegex(@"@import '(Tickets\.Web\.Client\.[^']+\.css)'")]
    private static partial Regex ClientBundle();
}
