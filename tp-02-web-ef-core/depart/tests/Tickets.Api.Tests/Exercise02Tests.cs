using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise02;

public class LifetimeTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private async Task<(string[] Singleton, string[] Scoped, string[] Transient)> CallAsync(HttpClient client)
    {
        var response = await client.GetAsync("/lifetimes", TestContext.Current.CancellationToken);
        Assert.True(response.IsSuccessStatusCode, $"GET /lifetimes answered {(int)response.StatusCode}: are the three services registered?");
        var json = await response.ReadJsonAsync();
        string[] Ids(string name) => [.. json[name]!.AsArray().Select(id => id!.GetValue<string>())];
        return (Ids("singleton"), Ids("scoped"), Ids("transient"));
    }

    [Fact]
    public async Task Singleton_IsTheSameEverywhere()
    {
        var client = factory.CreateClient();
        var first = await CallAsync(client);
        var second = await CallAsync(client);

        Assert.Equal(first.Singleton[0], first.Singleton[1]);
        Assert.Equal(first.Singleton[0], second.Singleton[0]);
    }

    [Fact]
    public async Task Scoped_IsShared_WithinOneRequest_Only()
    {
        var client = factory.CreateClient();
        var first = await CallAsync(client);
        var second = await CallAsync(client);

        Assert.Equal(first.Scoped[0], first.Scoped[1]);
        Assert.NotEqual(first.Scoped[0], second.Scoped[0]);
    }

    [Fact]
    public async Task Transient_IsNew_EachTime()
    {
        var result = await CallAsync(factory.CreateClient());

        Assert.NotEqual(result.Transient[0], result.Transient[1]);
    }
}
