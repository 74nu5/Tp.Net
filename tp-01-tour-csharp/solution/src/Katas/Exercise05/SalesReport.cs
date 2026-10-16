using Katas.Exercise03;

namespace Katas.Exercise05;

public sealed record CustomerSpending(string Name, decimal Spent);

public sealed record MonthlyRevenue(int Year, int Month, decimal Revenue);

public sealed record ProductSales(string Sku, int Quantity);

/// <summary>
/// Exercise 05: LINQ. Each method is a single query over <see cref="Order"/> objects.
/// </summary>
/// <remarks>
/// Java Streams equivalents: Where = filter, Select = map, SelectMany = flatMap, GroupBy = groupingBy,
/// OrderByDescending + Take = sorted(reverseOrder()) + limit, ToList = collect(toList()).
/// </remarks>
public sealed class SalesReport(IEnumerable<Order> orders)
{
    // Materialise once: the source could be a lazy sequence that would be re-evaluated by every query.
    private readonly IReadOnlyList<Order> _orders = [.. orders];

    /// <summary>1. Sum of all order totals.</summary>
    public decimal TotalRevenue() => _orders.Sum(order => order.Total);

    /// <summary>2. Number of orders delivered to a given country.</summary>
    public int CountOrdersTo(string country) => _orders.Count(order => order.Country == country);

    /// <summary>3. Average order value, rounded to 2 decimals; 0 when there is no order.</summary>
    public decimal AverageOrderValue() =>
        _orders.Count == 0 ? 0m : Math.Round(_orders.Average(order => order.Total), 2);

    /// <summary>4. The <paramref name="count"/> customers who spent the most, biggest spender first.</summary>
    public IReadOnlyList<CustomerSpending> TopCustomers(int count) =>
        [.. _orders
            .GroupBy(order => order.Customer)
            .Select(group => new CustomerSpending(group.Key.Name, group.Sum(order => order.Total)))
            .OrderByDescending(spending => spending.Spent)
            .Take(count)];

    /// <summary>5. Revenue per calendar month, in chronological order.</summary>
    public IReadOnlyList<MonthlyRevenue> RevenueByMonth() =>
        [.. _orders
            .GroupBy(order => (order.Date.Year, order.Date.Month))
            .Select(group => new MonthlyRevenue(group.Key.Year, group.Key.Month, group.Sum(order => order.Total)))
            .OrderBy(month => month.Year)
            .ThenBy(month => month.Month)];

    /// <summary>6. Revenue per product category.</summary>
    public IReadOnlyDictionary<string, decimal> RevenueByCategory() =>
        _orders
            .SelectMany(order => order.Lines)
            .GroupBy(line => line.Product.Category)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.Total));

    /// <summary>7. The <paramref name="count"/> best-selling products by quantity; ties are broken by SKU.</summary>
    public IReadOnlyList<ProductSales> BestSellingProducts(int count) =>
        [.. _orders
            .SelectMany(order => order.Lines)
            .GroupBy(line => line.Product.Sku)
            .Select(group => new ProductSales(group.Key, group.Sum(line => line.Quantity)))
            .OrderByDescending(sales => sales.Quantity)
            .ThenBy(sales => sales.Sku)
            .Take(count)];

    /// <summary>8. Names of the customers who never ordered, sorted alphabetically.</summary>
    public IReadOnlyList<string> CustomersWithoutOrders(IEnumerable<Customer> allCustomers) =>
        [.. allCustomers
            .ExceptBy(_orders.Select(order => order.Customer), customer => customer)
            .Select(customer => customer.Name)
            .Order()];

    /// <summary>9. Distinct delivery countries, sorted.</summary>
    public IReadOnlyList<string> CountriesServed() =>
        [.. _orders.Select(order => order.Country).Distinct().Order()];

    /// <summary>10. The order with the highest total, or null when there is none.</summary>
    public Order? LargestOrder() => _orders.MaxBy(order => order.Total);
}
