namespace Katas.Exercise03;

/// <summary>A product of the catalogue. Provided as an example of a positional record.</summary>
public sealed record Product(string Sku, string Name, string Category, decimal UnitPrice, bool IsFragile = false);

/// <summary>A line of an order. Provided.</summary>
public sealed record OrderLine(Product Product, int Quantity)
{
    public decimal Total => Product.UnitPrice * Quantity;
}

/// <summary>
/// An order: it has an identity and changes over time, so it stays a plain class.
/// Two orders with the same lines are still two different orders.
/// </summary>
public sealed class Order(int id, Customer customer, DateOnly date, string country)
{
    private readonly List<OrderLine> _lines = [];

    public int Id { get; } = id;

    public Customer Customer { get; } = customer;

    public DateOnly Date { get; } = date;

    /// <summary>ISO 3166 country code of the delivery address, e.g. "FR".</summary>
    public string Country { get; } = country;

    // Expose a read-only view: callers can read the lines but must go through AddLine to change them.
    public IReadOnlyList<OrderLine> Lines => _lines;

    // TODO Exercise 03: sum of the lines.
    public decimal Total => throw new NotImplementedException("TODO Exercise 03");

    public Order AddLine(Product product, int quantity)
    {
        // TODO Exercise 03: reject a quantity of zero or less with an ArgumentOutOfRangeException.
        _lines.Add(new OrderLine(product, quantity));
        return this;
    }
}
