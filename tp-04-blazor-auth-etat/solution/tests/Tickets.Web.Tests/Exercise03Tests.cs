using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Web.Client.Api;
using Tickets.Web.Components.Layout;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Exercise03;

public class LoginFlowTests(WebAppFactory app) : IClassFixture<WebAppFactory>
{
    [Fact]
    public async Task NewTicketPage_SendsAnonymousUsersToTheLoginPage()
    {
        var response = await app.CreateBrowser().GetAsync("/tickets/new", Xunit.TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/account/login", response.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task OnceLoggedIn_TheNewTicketPageOpens()
    {
        var browser = app.CreateBrowser();

        var login = await app.LogInAsync(browser, "alice", "demo");

        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
        Assert.Contains("<h1>New ticket</h1>", await app.GetHtmlAsync("/tickets/new", browser));
    }

    [Fact]
    public async Task WrongPassword_KeepsTheUserAnonymous()
    {
        var browser = app.CreateBrowser();

        await app.LogInAsync(browser, "alice", "wrong");

        var response = await browser.GetAsync("/tickets/new", Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }
}

public class NavMenuTests : ComponentTest
{
    [Fact]
    public void AnonymousUser_SeesTheLoginLink_AndNoNewTicketLink()
    {
        AddAuthorization().SetNotAuthorized();

        var menu = Render<NavMenu>();

        Assert.NotNull(menu.Find("a[href='account/login']"));
        Assert.Empty(menu.FindAll("a[href='tickets/new']"));
    }

    [Fact]
    public void LoggedInUser_SeesTheirName_TheNewTicketLink_AndALogoutButton()
    {
        AddAuthorization().SetAuthorized("alice");

        var menu = Render<NavMenu>();

        Assert.Contains("alice", menu.Markup);
        Assert.NotNull(menu.Find("a[href='tickets/new']"));
        Assert.NotNull(menu.Find("form[action='account/logout'] button"));
    }

    [Fact]
    public void Administrator_IsShownAsSuch()
    {
        AddAuthorization().SetAuthorized("admin").SetRoles("admin");

        var menu = Render<NavMenu>();

        Assert.Contains("administrator", menu.Markup);
    }
}

public class ApiTokenTests
{
    [Fact]
    public async Task CallsToTheApi_CarryTheConfiguredToken()
    {
        var capture = new CapturingHandler();
        await using var app = new WebAppFactory();
        app.Settings["TicketsApi:Token"] = "test-token";
        app.ExtraServices = services =>
        {
            // Back to the real typed client, whose last handler records the request instead of sending it.
            services.AddHttpClient<ITicketsClient, TicketsClient>().ConfigurePrimaryHttpMessageHandler(() => capture);
        };
        using var scope = app.Server.Services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ITicketsClient>().GetTicketsAsync(Xunit.TestContext.Current.CancellationToken);

        Assert.Equal("Bearer test-token", capture.Authorization);
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public string? Authorization { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Authorization = request.Headers.Authorization?.ToString();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") });
        }
    }
}
