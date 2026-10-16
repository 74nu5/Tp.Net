namespace Katas.Exercise03;

/// <summary>
/// A customer, identified by its data: two customers with the same name and email are the same customer.
/// </summary>
/// <remarks>
/// TODO Exercise 03: make two customers with the same data equal, give them a readable ToString(),
/// force callers to set the name, and say in the type that the email is optional.
/// Then look at the warnings of the build.
/// </remarks>
public class Customer
{
    public string Name { get; set; }

    public string Email { get; set; }
}
