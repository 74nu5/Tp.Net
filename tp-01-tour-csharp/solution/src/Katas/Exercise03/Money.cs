using System.Globalization;

namespace Katas.Exercise03;

/// <summary>
/// An amount in a given currency.
/// </summary>
/// <remarks>
/// A <c>readonly record struct</c>: a small immutable value, compared by value, stored inline (no separate allocation, unless boxed).
/// Java equivalent: a <c>record</c>; Rust equivalent: <c>#[derive(Clone, PartialEq)] struct</c> (not <c>Copy</c>: the currency is a <c>String</c>).
/// </remarks>
public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot add {left.Currency} and {right.Currency}.");
        }

        return left with { Amount = left.Amount + right.Amount };
    }

    // Invariant culture: "12.50 EUR" everywhere, not "12,50 EUR" on a French machine.
    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Amount:0.00} {Currency}");
}
