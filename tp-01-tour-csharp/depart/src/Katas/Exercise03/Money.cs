namespace Katas.Exercise03;

/// <summary>
/// An amount in a given currency.
/// </summary>
/// <remarks>
/// TODO Exercise 03: this class compares by reference and lives on the heap.
/// Turn it into a small immutable value compared by value, then implement the + operator.
/// </remarks>
public class Money
{
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public string Currency { get; }

    /// <summary>Adds two amounts of the same currency; throws <see cref="InvalidOperationException"/> otherwise.</summary>
    public static Money operator +(Money left, Money right) =>
        throw new NotImplementedException("TODO Exercise 03");

    /// <summary>Formats as "12.50 EUR", whatever the machine's regional settings are.</summary>
    public override string ToString() => throw new NotImplementedException("TODO Exercise 03");
}
