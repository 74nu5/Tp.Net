using Katas.Exercise03;
using Katas.Exercise04;
using Katas.Exercise06;
using Katas.Tests.Support;

namespace Katas.Tests.Exercise07;

/// <summary>
/// Exercise 07: tests written by the apprentice. This is one possible answer.
/// </summary>
public class MyTests
{
    // A [Theory] runs once per [InlineData]: the equivalent of JUnit 5's @ParameterizedTest.
    [Theory]
    [InlineData("FR", 4.90)]
    [InlineData("IT", 9.90)]
    [InlineData("JP", 19.90)]
    public void Cost_OfASmallOrder_DependsOnTheCountry(string country, double expected)
    {
        var order = new Order(1, new Customer { Name = "Test" }, new DateOnly(2026, 10, 19), country)
            .AddLine(new Product("X", "Thing", "Misc", 10m), 1);

        Assert.Equal((decimal)expected, Shipping.Cost(order));
    }

    [Fact]
    public void Money_RefusesToMixCurrencies()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new Money(1m, "EUR") + new Money(1m, "GBP"));

        Assert.Contains("GBP", exception.Message);
    }

    // Async tests return Task; xUnit awaits them.
    [Fact]
    public async Task FetchSizes_OfAnEmptyList_IsEmpty()
    {
        var fetcher = new PageFetcher(new HttpClient(new FakeHttpHandler()));

        var sizes = await fetcher.FetchSizesAsync([], TestContext.Current.CancellationToken);

        Assert.Empty(sizes);
    }
}
