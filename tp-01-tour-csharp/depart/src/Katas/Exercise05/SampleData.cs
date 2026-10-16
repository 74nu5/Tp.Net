using Katas.Exercise03;

namespace Katas.Exercise05;

/// <summary>A small, fixed data set for the LINQ exercise. Provided: do not change it.</summary>
public static class SampleData
{
    public static readonly Customer Alice = new() { Name = "Alice", Email = "alice@example.com" };
    public static readonly Customer Bruno = new() { Name = "Bruno", Email = "bruno@example.org" };
    public static readonly Customer Chloe = new() { Name = "Chloé" };
    public static readonly Customer Dmitri = new() { Name = "Dmitri", Email = "dmitri@example.net" };
    public static readonly Customer Emma = new() { Name = "Emma", Email = "emma@example.com" };

    public static readonly IReadOnlyList<Customer> Customers = [Alice, Bruno, Chloe, Dmitri, Emma];

    public static readonly Product Keyboard = new("KB-01", "Mechanical keyboard", "Hardware", 89.00m);
    public static readonly Product Mouse = new("MS-01", "Wireless mouse", "Hardware", 29.90m);
    public static readonly Product Screen = new("SC-27", "27-inch screen", "Hardware", 249.00m, IsFragile: true);
    public static readonly Product Book = new("BK-CS", "C# in Depth", "Books", 45.00m);
    public static readonly Product Mug = new("MG-01", "Developer mug", "Goodies", 12.50m, IsFragile: true);
    public static readonly Product Sticker = new("ST-01", "Sticker pack", "Goodies", 4.00m);
    public static readonly Product Course = new("CO-NET", "Online .NET course", "Training", 120.00m);

    public static readonly IReadOnlyList<Product> Products = [Keyboard, Mouse, Screen, Book, Mug, Sticker, Course];

    public static IReadOnlyList<Order> CreateOrders() =>
    [
        new Order(1, Alice, new DateOnly(2026, 9, 2), "FR").AddLine(Keyboard, 1).AddLine(Mouse, 1),
        new Order(2, Bruno, new DateOnly(2026, 9, 5), "BE").AddLine(Book, 2),
        new Order(3, Alice, new DateOnly(2026, 9, 18), "FR").AddLine(Mug, 4).AddLine(Sticker, 3),
        new Order(4, Chloe, new DateOnly(2026, 9, 21), "CA").AddLine(Screen, 1),
        new Order(5, Dmitri, new DateOnly(2026, 9, 30), "DE").AddLine(Course, 1).AddLine(Book, 1),
        new Order(6, Bruno, new DateOnly(2026, 10, 1), "BE").AddLine(Sticker, 10),
        new Order(7, Alice, new DateOnly(2026, 10, 3), "FR").AddLine(Screen, 2).AddLine(Keyboard, 1),
        new Order(8, Chloe, new DateOnly(2026, 10, 7), "CA").AddLine(Mouse, 2).AddLine(Mug, 1),
        new Order(9, Dmitri, new DateOnly(2026, 10, 9), "DE").AddLine(Sticker, 2),
        new Order(10, Bruno, new DateOnly(2026, 10, 12), "FR").AddLine(Course, 1),
    ];
}
