namespace Katas.Exercise03;

/// <summary>
/// Exercise 03, nullability: the starting version compiles with CS8602 warnings and crashes on null.
/// </summary>
public static class CustomerDirectory
{
    public static string DisplayName(Customer? customer) =>
        customer?.Name.ToUpperInvariant() ?? "ANONYMOUS";

    public static string EmailDomain(Customer customer) =>
        customer.Email is { } email && email.IndexOf('@') is var at and >= 0
            ? email[(at + 1)..]
            : "";
}
