using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise03;

public class SettingsTests
{
    [Fact]
    public async Task Settings_ComeFromAppSettingsJson()
    {
        await using var factory = new TicketsApiFactory();

        var json = await (await factory.CreateClient().GetAsync("/settings", TestContext.Current.CancellationToken)).ReadJsonAsync();

        Assert.Equal(25, json["pageSize"]!.GetValue<int>());
        Assert.Equal("support@example.com", json["supportEmail"]!.GetValue<string>());
    }

    [Fact]
    public async Task Settings_CanBeOverridden()
    {
        // Same effect as the environment variable Tickets__PageSize=5.
        await using var factory = TicketsApiFactory.With(("Tickets:PageSize", "5"));

        var json = await (await factory.CreateClient().GetAsync("/settings", TestContext.Current.CancellationToken)).ReadJsonAsync();

        Assert.Equal(5, json["pageSize"]!.GetValue<int>());
    }

    [Fact]
    public async Task InvalidSettings_StopTheApplicationAtStartup()
    {
        await using var factory = TicketsApiFactory.With(("Tickets:PageSize", "0"));

        var exception = Assert.ThrowsAny<Exception>(() => factory.CreateClient());

        Assert.Contains("PageSize", exception.ToString());
    }
}
