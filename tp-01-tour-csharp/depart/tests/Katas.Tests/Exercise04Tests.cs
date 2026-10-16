using Katas.Exercise03;
using Katas.Exercise04;
using Katas.Exercise05;

namespace Katas.Tests.Exercise04;

public class ShippingTests
{
    private static Order OrderNumber(int id) => SampleData.CreateOrders().Single(order => order.Id == id);

    [Theory]
    [InlineData(1, 0)]       // France, 118.90 EUR: free
    [InlineData(4, 5)]       // Canada, 249.00 EUR, fragile screen: free + 5.00
    [InlineData(3, 9.90)]    // France, 62.00 EUR, fragile mugs: 4.90 + 5.00
    [InlineData(2, 9.90)]    // Belgium, 90.00 EUR
    [InlineData(9, 9.90)]    // Germany, 8.00 EUR
    [InlineData(8, 24.90)]   // Canada, 72.30 EUR, fragile mug: 19.90 + 5.00
    public void Cost_FollowsTheRules(int orderId, double expected) =>
        Assert.Equal((decimal)expected, Shipping.Cost(OrderNumber(orderId)));

    [Fact]
    public void Cost_OutsideTheEuropeanUnion_Is19_90()
    {
        var order = new Order(42, SampleData.Emma, new DateOnly(2026, 10, 19), "US").AddLine(SampleData.Sticker, 1);

        Assert.Equal(19.90m, Shipping.Cost(order));
    }
}

public class DescribeTests
{
    public static TheoryData<object?, string> Values => new()
    {
        { null, "nothing" },
        { -3, "negative number" },
        { 42, "number 42" },
        { "", "empty text" },
        { "hello", "text of 5 characters" },
        { new Money(0m, "EUR"), "no money" },
        { new Money(12.5m, "EUR"), "12.50 EUR" },
        { Array.Empty<int>(), "empty array" },
        { new[] { 7 }, "array of 7" },
        { new[] { 1, 2, 3 }, "array from 1 to 3" },
        { 3.14, "unknown" },
    };

    [Theory]
    [MemberData(nameof(Values))]
    public void Describe_RecognisesTheShapeOfTheValue(object? value, string expected) =>
        Assert.Equal(expected, Shipping.Describe(value));
}
