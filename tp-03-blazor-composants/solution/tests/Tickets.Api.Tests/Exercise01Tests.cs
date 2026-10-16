using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise01;

public class RequestTimingTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    [Fact]
    public async Task EveryResponse_CarriesItsDuration()
    {
        var response = await factory.CreateClient().GetAsync("/", TestContext.Current.CancellationToken);

        Assert.True(response.Headers.TryGetValues("X-Request-Duration-Ms", out var values), "The X-Request-Duration-Ms header is missing.");
        Assert.True(long.TryParse(values.Single(), out var milliseconds) && milliseconds >= 0, "The header must be a number of milliseconds.");
    }

    [Fact]
    public async Task EvenA404_CarriesItsDuration()
    {
        var response = await factory.CreateClient().GetAsync("/does-not-exist", TestContext.Current.CancellationToken);

        Assert.True(response.Headers.Contains("X-Request-Duration-Ms"));
    }
}
