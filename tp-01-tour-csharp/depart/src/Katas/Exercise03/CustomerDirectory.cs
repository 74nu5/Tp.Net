namespace Katas.Exercise03;

/// <summary>
/// Exercise 03, nullability. TODO: this code crashes on null. Fix it until the build shows no warning.
/// </summary>
public static class CustomerDirectory
{
    /// <summary>The name in upper case, or "ANONYMOUS" when there is no customer.</summary>
    public static string DisplayName(Customer? customer) => customer.Name.ToUpperInvariant();

    /// <summary>The part of the email after the @, or "" when the customer has no email.</summary>
    public static string EmailDomain(Customer customer) => customer.Email.Split('@')[1];
}
