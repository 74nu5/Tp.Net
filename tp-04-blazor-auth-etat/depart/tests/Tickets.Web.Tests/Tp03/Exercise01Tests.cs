using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Tp03.Exercise01;

public class AboutPageTests(WebAppFactory app) : IClassFixture<WebAppFactory>
{
    [Fact]
    public async Task AboutPage_IsServedAtSlashAbout()
    {
        var html = await app.GetHtmlAsync("/about");

        Assert.Contains("<h1>About</h1>", html);
    }

    [Fact]
    public async Task Menu_LinksToTheAboutPage()
    {
        var html = await app.GetHtmlAsync("/");

        Assert.Contains("href=\"about\"", html);
    }
}
