using Microsoft.AspNetCore.Mvc.Testing;

namespace Tickets.Web.Tests.Support;

/// <summary>Starts the whole Blazor Web App in memory, to request pages as a browser would.</summary>
public sealed class WebAppFactory : WebApplicationFactory<Program>
{
    public async Task<string> GetHtmlAsync(string path)
    {
        var response = await CreateClient().GetAsync(path, Xunit.TestContext.Current.CancellationToken);
        Assert.True(response.IsSuccessStatusCode, $"GET {path} answered {(int)response.StatusCode}.");
        return await response.Content.ReadAsStringAsync(Xunit.TestContext.Current.CancellationToken);
    }
}
