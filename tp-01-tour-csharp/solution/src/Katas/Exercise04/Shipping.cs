using Katas.Exercise03;

namespace Katas.Exercise04;

/// <summary>
/// Exercise 04: pattern matching and switch expressions.
/// </summary>
public static class Shipping
{
    private static readonly HashSet<string> EuropeanUnion =
        ["AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "GR", "HR", "HU", "IE", "IT",
         "LT", "LU", "LV", "MT", "NL", "PL", "PT", "RO", "SE", "SI", "SK"];

    /// <summary>
    /// Shipping cost of an order: free from 100 EUR, 4.90 in France, 9.90 elsewhere in the EU,
    /// 19.90 in the rest of the world; fragile items add 5.00, even when shipping is free.
    /// </summary>
    public static decimal Cost(Order order)
    {
        // A switch expression with property and relational patterns; arms are tried top to bottom.
        var baseCost = order switch
        {
            { Total: >= 100m } => 0m,
            { Country: "FR" } => 4.90m,
            { Country: var country } when EuropeanUnion.Contains(country) => 9.90m,
            _ => 19.90m,
        };

        var fragileSurcharge = order.Lines.Any(line => line.Product is { IsFragile: true }) ? 5m : 0m;
        return baseCost + fragileSurcharge;
    }

    /// <summary>
    /// Describes any value:
    /// null → "nothing"; a negative int → "negative number"; another int → "number 42";
    /// "" → "empty text"; another string → "text of 5 characters";
    /// a Money of 0 → "no money"; another Money → its ToString(), e.g. "12.50 EUR";
    /// an empty int[] → "empty array"; one element → "array of 7"; more → "array from 1 to 3";
    /// anything else → "unknown".
    /// </summary>
    /// <remarks>Practises type, relational, property and list patterns.</remarks>
    public static string Describe(object? value) => value switch
    {
        null => "nothing",
        int and < 0 => "negative number",
        int n => $"number {n}",
        string { Length: 0 } => "empty text",
        string text => $"text of {text.Length} characters",
        Money { Amount: 0 } => "no money",
        Money money => $"{money}",
        int[] and [] => "empty array",
        int[] and [var first, .., var last] => $"array from {first} to {last}",
        int[] and [var single] => $"array of {single}",
        _ => "unknown",
    };
}
