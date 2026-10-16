using Katas.Exercise03;

namespace Katas.Tests.Exercise03;

public class MoneyTests
{
    [Fact]
    public void Money_IsAValueType() => Assert.True(typeof(Money).IsValueType);

    [Fact]
    public void TwoAmounts_WithTheSameValue_AreEqual() =>
        Assert.Equal(new Money(10m, "EUR"), new Money(10m, "EUR"));

    [Fact]
    public void Adding_SameCurrency_SumsTheAmounts() =>
        Assert.Equal(new Money(15.5m, "EUR"), new Money(10m, "EUR") + new Money(5.5m, "EUR"));

    [Fact]
    public void Adding_DifferentCurrencies_Throws() =>
        Assert.Throws<InvalidOperationException>(() => new Money(10m, "EUR") + new Money(5m, "USD"));
}

public class CustomerTests
{
    [Fact]
    public void TwoCustomers_WithTheSameData_AreEqual()
    {
        var first = new Customer { Name = "Alice", Email = "alice@example.com" };
        var second = new Customer { Name = "Alice", Email = "alice@example.com" };

        Assert.Equal(first, second);
    }

    [Fact]
    public void Customer_HasAReadableToString() =>
        Assert.Contains("Name = Alice", new Customer { Name = "Alice" }.ToString());

    [Fact]
    public void Email_IsOptional() => Assert.Null(new Customer { Name = "Chloé" }.Email);
}

public class OrderTests
{
    private static readonly Customer Alice = new() { Name = "Alice" };
    private static readonly Product Book = new("BK", "Book", "Books", 45m);
    private static readonly Product Pen = new("PN", "Pen", "Office", 2.5m);

    [Fact]
    public void Total_SumsTheLines()
    {
        var order = new Order(1, Alice, new DateOnly(2026, 10, 19), "FR").AddLine(Book, 2).AddLine(Pen, 4);

        Assert.Equal(100m, order.Total);
    }

    [Fact]
    public void AddLine_RejectsANonPositiveQuantity()
    {
        var order = new Order(1, Alice, new DateOnly(2026, 10, 19), "FR");

        Assert.Throws<ArgumentOutOfRangeException>(() => order.AddLine(Book, 0));
    }

    [Fact]
    public void TwoOrders_WithTheSameData_AreStillDifferentOrders()
    {
        var first = new Order(1, Alice, new DateOnly(2026, 10, 19), "FR");
        var second = new Order(1, Alice, new DateOnly(2026, 10, 19), "FR");

        Assert.NotEqual(first, second);
    }
}

public class CustomerDirectoryTests
{
    [Fact]
    public void DisplayName_OfNobody_IsAnonymous() => Assert.Equal("ANONYMOUS", CustomerDirectory.DisplayName(null));

    [Fact]
    public void DisplayName_IsUpperCase() =>
        Assert.Equal("ALICE", CustomerDirectory.DisplayName(new Customer { Name = "Alice" }));

    [Fact]
    public void EmailDomain_ReturnsThePartAfterTheAt() =>
        Assert.Equal("example.com", CustomerDirectory.EmailDomain(new Customer { Name = "Alice", Email = "alice@example.com" }));

    [Fact]
    public void EmailDomain_WithoutEmail_IsEmpty() =>
        Assert.Equal("", CustomerDirectory.EmailDomain(new Customer { Name = "Chloé" }));
}
