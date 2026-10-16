using Katas.Exercise02;

namespace Katas.Tests.Exercise02;

public class TextFormatterTests
{
    [Fact]
    public void DescribeDuration_KeepsTheTwoLargestUnits() =>
        Assert.Equal("2 hours, 5 minutes", TextFormatter.DescribeDuration(TimeSpan.FromMinutes(125)));

    [Fact]
    public void DescribeDuration_HandlesASingleUnit() =>
        Assert.Equal("3 days", TextFormatter.DescribeDuration(TimeSpan.FromDays(3)));

    [Theory]
    [InlineData("OrderLineCount", "Order line count")]
    [InlineData("customer_email", "Customer email")]
    public void ToSentence_SplitsIdentifiers(string identifier, string expected) =>
        Assert.Equal(expected, TextFormatter.ToSentence(identifier));
}
