namespace Katas.Exercise03;

/// <summary>
/// Exercise 03, nullability: the starting version compiles with a CS8602 warning (a second one once Email becomes string?) and crashes on null.
/// </summary>
public static class CustomerDirectory
{
    /// <summary>The name in upper case, or "ANONYMOUS" when there is no customer.</summary>
    public static string DisplayName(Customer? customer) =>
        customer?.Name.ToUpperInvariant() ?? "ANONYMOUS";

    /// <summary>The part of the email after the @, or "" when the customer has no email.</summary>
    public static string EmailDomain(Customer customer) =>
        customer.Email is { } email && email.IndexOf('@') is var at and >= 0
            ? email[(at + 1)..]
            : "";
}
