using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Exercise04;

/// <summary>
/// An interactive component leaves a marker in the prerendered HTML: &lt;!--Blazor:{"type":"server",…}--&gt;.
/// The marker tells blazor.web.js where and how to make the component interactive.
/// </summary>
public class RenderModeTests(WebAppFactory app) : IClassFixture<WebAppFactory>
{
    [Fact]
    public async Task StaticPage_IsPlainHtml()
    {
        var html = await app.GetHtmlAsync("/render-modes/static");

        Assert.Contains("Static", html);
        Assert.DoesNotContain("<!--Blazor:{", html);
    }

    [Theory]
    [InlineData("/render-modes/server", "server")]
    [InlineData("/render-modes/webassembly", "webassembly")]
    [InlineData("/render-modes/auto", "auto")]
    public async Task InteractivePages_UseTheExpectedRenderMode(string path, string type)
    {
        var html = await app.GetHtmlAsync(path);

        Assert.Contains($"<!--Blazor:{{\"type\":\"{type}\"", html);
    }
}
