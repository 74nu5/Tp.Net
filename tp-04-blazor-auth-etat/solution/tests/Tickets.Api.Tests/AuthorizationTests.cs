using System.Net;
using System.Net.Http.Json;
using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Authorization;

/// <summary>TP-04, exercise 02: reading is public, writing needs a user, deleting needs an administrator.</summary>
[Collection(DatabaseCollection.Name)]
public class TicketAuthorizationTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private CancellationToken Token => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Reading_IsOpenToEveryone()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/tickets", Token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Creating_WithoutToken_Is401()
    {
        var response = await factory.CreateAnonymousClient().PostAsJsonAsync("/tickets", new { title = "Anonymous" }, Token);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Creating_AsAUser_IsAllowed()
    {
        var response = await factory.CreateClientFor("alice").PostAsJsonAsync("/tickets", new { title = HttpExtensions.UniqueTitle("By Alice") }, Token);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [InlineData("PUT", "/tickets/1")]
    [InlineData("POST", "/tickets/1/comments")]
    [InlineData("POST", "/tickets/1/close")]
    public async Task OtherWrites_WithoutToken_Are401(string method, string path)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), path) { Content = JsonContent.Create(new { title = "x", author = "x", text = "x" }) };

        var response = await factory.CreateAnonymousClient().SendAsync(request, Token);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Deleting_AsAUser_Is403()
    {
        var id = (await factory.CreateClient().CreateTicketAsync(HttpExtensions.UniqueTitle("Keep me")))["id"];

        var response = await factory.CreateClientFor("alice").DeleteAsync($"/tickets/{id}", Token);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Deleting_AsAnAdministrator_IsAllowed()
    {
        var id = (await factory.CreateClient().CreateTicketAsync(HttpExtensions.UniqueTitle("Delete me")))["id"];

        var response = await factory.CreateClientFor("carol", "admin").DeleteAsync($"/tickets/{id}", Token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
