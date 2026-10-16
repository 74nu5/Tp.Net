using System.Net;
using System.Net.Http.Headers;
using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Authentication;

/// <summary>TP-04, exercise 01: the API knows who calls it.</summary>
public class MeTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private CancellationToken Token => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Me_WithoutToken_Is401()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/me", Token);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_ReturnsTheNameFromTheToken()
    {
        var response = await factory.CreateClientFor("alice").GetAsync("/me", Token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var me = await response.ReadJsonAsync();
        Assert.Equal("alice", me["name"]!.GetValue<string>());
        Assert.False(me["isAdmin"]!.GetValue<bool>());
    }

    [Fact]
    public async Task Me_ReturnsTheRolesFromTheToken()
    {
        var me = await (await factory.CreateClientFor("bob", "admin").GetAsync("/me", Token)).ReadJsonAsync();

        Assert.Contains("admin", me["roles"]!.AsArray().Select(role => role!.GetValue<string>()));
        Assert.True(me["isAdmin"]!.GetValue<bool>());
    }

    [Fact]
    public async Task Me_WithAForgedToken_Is401()
    {
        var client = factory.CreateAnonymousClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.Forged("mallory"));

        var response = await client.GetAsync("/me", Token);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
