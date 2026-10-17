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
    // Provided. Why copy the sequence instead of keeping it? See the question at the end of exercise 5.
    private readonly IReadOnlyList<Order> _orders = [.. orders];

    /// <summary>1. Sum of all order totals.</summary>
    public decimal TotalRevenue() => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>2. Number of orders delivered to a given country.</summary>
    public int CountOrdersTo(string country) => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>3. Average order value, rounded to 2 decimals; 0 when there is no order.</summary>
    public decimal AverageOrderValue() => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>4. The <paramref name="count"/> customers who spent the most, biggest spender first.</summary>
    public IReadOnlyList<CustomerSpending> TopCustomers(int count) => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>5. Revenue per calendar month, in chronological order.</summary>
    public IReadOnlyList<MonthlyRevenue> RevenueByMonth() => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>6. Revenue per product category.</summary>
    public IReadOnlyDictionary<string, decimal> RevenueByCategory() => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>7. The <paramref name="count"/> best-selling products by quantity; ties are broken by SKU.</summary>
    public IReadOnlyList<ProductSales> BestSellingProducts(int count) => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>8. Names of the customers who never ordered, sorted alphabetically.</summary>
    public IReadOnlyList<string> CustomersWithoutOrders(IEnumerable<Customer> allCustomers) => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>9. Distinct delivery countries, sorted.</summary>
    public IReadOnlyList<string> CountriesServed() => throw new NotImplementedException("TODO Exercise 05");

    /// <summary>10. The order with the highest total, or null when there is none.</summary>
    public Order? LargestOrder() => throw new NotImplementedException("TODO Exercise 05");
}
