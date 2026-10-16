using System.Reflection;
using Microsoft.AspNetCore.Components;
using Tickets.Web.Client.Api;
using Tickets.Web.Client.Components;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Exercise06;

// The library does not exist in depart/: these tests find it by name, so that they compile before it is created.
public class ComponentLibraryTests : ComponentTest
{
    private static Type? FindBadge()
    {
        try
        {
            return Assembly.Load("Tickets.Components").GetType("Tickets.Components.Badge");
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    [Fact]
    public void TheLibrary_ContainsABadgeComponent()
    {
        var badge = FindBadge();

        Assert.True(badge is not null, "No Tickets.Components.Badge: create the Tickets.Components library and reference it from Tickets.Web.Client.");
        Assert.True(typeof(IComponent).IsAssignableFrom(badge));
    }

    [Fact]
    public void PriorityBadge_StillRendersTheSameBadge()
    {
        var badge = Render<PriorityBadge>(parameters => parameters.Named("Priority", TicketPriority.High));

        var span = badge.Find("span.badge");
        Assert.Equal("High", span.TextContent.Trim());
        Assert.Contains("badge-high", span.ClassList);
    }

    [Fact]
    public void PriorityBadge_IsDrawnByTheLibraryBadge_WithItsScopedStyles()
    {
        var badge = Render<PriorityBadge>(parameters => parameters.Named("Priority", TicketPriority.Low));

        // CSS isolation adds an attribute b-xxxxxxxx to the elements of a component that has a .razor.css file.
        var span = badge.Find("span.badge");
        Assert.True(span.Attributes.Any(attribute => attribute.Name.StartsWith("b-", StringComparison.Ordinal)),
            "The badge has no scope attribute: give Badge its own Badge.razor.css in the library.");
    }
}

public class LibraryAssetsTests(WebAppFactory app) : IClassFixture<WebAppFactory>
{
    [Fact]
    public async Task TheLibraryStyles_AreServedUnderContent()
    {
        var bundle = await app.GetHtmlAsync("/Tickets.Web.styles.css");

        Assert.Contains("_content/Tickets.Components/", bundle);
    }
}
