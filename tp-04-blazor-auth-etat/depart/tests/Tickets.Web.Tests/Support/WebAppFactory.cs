using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Web.Client.Api;

namespace Tickets.Web.Tests.Support;

/// <summary>
/// Starts the whole Blazor Web App in memory, to request pages as a browser would.
/// The API is replaced by <see cref="Api"/>, so no API needs to run.
/// </summary>
public sealed partial class WebAppFactory : WebApplicationFactory<Program>
{
    public FakeTicketsClient Api { get; } = new();

    /// <summary>Extra configuration values, set before the first client is created.</summary>
    public Dictionary<string, string?> Settings { get; } = [];

    /// <summary>Extra service registrations, applied after the application's own.</summary>
    public Action<IServiceCollection>? ExtraServices { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(Settings));
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<ITicketsClient>(Api);
            ExtraServices?.Invoke(services);
        });
    }

    /// <summary>A client that keeps cookies but does not follow redirects, to look at them.</summary>
    public HttpClient CreateBrowser() => CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public async Task<string> GetHtmlAsync(string path, HttpClient? client = null)
    {
        var response = await (client ?? CreateClient()).GetAsync(path, Xunit.TestContext.Current.CancellationToken);
        Assert.True(response.IsSuccessStatusCode, $"GET {path} answered {(int)response.StatusCode}.");
        return await response.Content.ReadAsStringAsync(Xunit.TestContext.Current.CancellationToken);
    }

    /// <summary>Logs in through the real login form, antiforgery token included. Returns the response to the POST.</summary>
    public async Task<HttpResponseMessage> LogInAsync(HttpClient browser, string userName, string password)
    {
        var form = await GetHtmlAsync("/account/login", browser);
        var token = AntiforgeryToken().Match(form);
        Assert.True(token.Success, "No antiforgery token in the login form.");

        return await browser.PostAsync("/account/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["_handler"] = "login",
            ["__RequestVerificationToken"] = token.Groups[1].Value,
            ["Input.UserName"] = userName,
            ["Input.Password"] = password,
        }), Xunit.TestContext.Current.CancellationToken);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\" value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryToken();
}
