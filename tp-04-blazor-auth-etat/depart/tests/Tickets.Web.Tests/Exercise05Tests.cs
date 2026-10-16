using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Web.Components.Pages;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Exercise05;

public partial class PersistentStateTests(WebAppFactory app) : IClassFixture<WebAppFactory>
{
    [Fact]
    public void TheTickets_AreAPersistentProperty()
    {
        var property = typeof(TicketList).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .SingleOrDefault(p => p.GetCustomAttribute<PersistentStateAttribute>() is not null);

        Assert.True(property is not null, "TicketList has no public property marked [PersistentState].");
    }

    [Fact]
    public async Task PrerenderedPage_CarriesTheTickets_ForTheInteractiveRender()
    {
        app.Api.Tickets.Add(FakeTicketsClient.Ticket(1, "Persisted printer"));

        var html = await app.GetHtmlAsync("/tickets");

        // The state is in an HTML comment, encrypted with the application's data protection keys.
        var payload = ServerState().Match(html);
        Assert.True(payload.Success, "No persisted component state in the page.");
        var protector = app.Services.GetRequiredService<IDataProtectionProvider>()
            .CreateProtector("Microsoft.AspNetCore.Components.Server.State");
        var state = JsonSerializer.Deserialize<Dictionary<string, string>>(
            protector.Unprotect(Convert.FromBase64String(payload.Groups[1].Value)))!;
        var values = state.Values.Select(value => Encoding.UTF8.GetString(Convert.FromBase64String(value)));

        Assert.Contains(values, value => value.Contains("Persisted printer"));
    }

    [GeneratedRegex("<!--Blazor-Server-Component-State:([A-Za-z0-9+/=_-]+)-->")]
    private static partial Regex ServerState();
}
