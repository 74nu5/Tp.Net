using Katas.Exercise05;

namespace Katas.Tests.Exercise05;

public class SalesReportTests
{
    private readonly SalesReport _report = new(SampleData.CreateOrders());

    [Fact]
    public void TotalRevenue() => Assert.Equal(1512.20m, _report.TotalRevenue());

    [Fact]
    public void CountOrdersTo() => Assert.Equal(4, _report.CountOrdersTo("FR"));

    [Fact]
    public void AverageOrderValue() => Assert.Equal(151.22m, _report.AverageOrderValue());

    [Fact]
    public void AverageOrderValue_WithoutOrders_IsZero() => Assert.Equal(0m, new SalesReport([]).AverageOrderValue());

    [Fact]
    public void TopCustomers() =>
        Assert.Equal(
            [new CustomerSpending("Alice", 767.90m), new CustomerSpending("Chloé", 321.30m), new CustomerSpending("Bruno", 250.00m)],
            _report.TopCustomers(3));

    [Fact]
    public void RevenueByMonth() =>
        Assert.Equal(
            [new MonthlyRevenue(2026, 9, 684.90m), new MonthlyRevenue(2026, 10, 827.30m)],
            _report.RevenueByMonth());

    [Fact]
    public void RevenueByCategory()
    {
        var revenue = _report.RevenueByCategory();

        Assert.Equal(4, revenue.Count);
        Assert.Equal(1014.70m, revenue["Hardware"]);
        Assert.Equal(135.00m, revenue["Books"]);
        Assert.Equal(122.50m, revenue["Goodies"]);
        Assert.Equal(240.00m, revenue["Training"]);
    }

    [Fact]
    public void BestSellingProducts() =>
        Assert.Equal(
            [new ProductSales("ST-01", 15), new ProductSales("MG-01", 5), new ProductSales("BK-CS", 3)],
            _report.BestSellingProducts(3));

    [Fact]
    public void CustomersWithoutOrders() => Assert.Equal(["Emma"], _report.CustomersWithoutOrders(SampleData.Customers));

    [Fact]
    public void CountriesServed() => Assert.Equal(["BE", "CA", "DE", "FR"], _report.CountriesServed());

    [Fact]
    public void LargestOrder() => Assert.Equal(7, _report.LargestOrder()?.Id);

    [Fact]
    public void LargestOrder_WithoutOrders_IsNull() => Assert.Null(new SalesReport([]).LargestOrder());
}
