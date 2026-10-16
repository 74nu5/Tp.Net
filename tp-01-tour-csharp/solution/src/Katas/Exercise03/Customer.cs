namespace Katas.Exercise03;

/// <summary>
/// A customer, identified by its data: two customers with the same name and email are the same customer.
/// </summary>
/// <remarks>
/// A <c>record</c> (class): value equality, a readable <c>ToString()</c> and <c>with</c> copies for free.
/// <c>required</c> forces callers to set <see cref="Name"/>; <c>string?</c> says the email is optional.
/// </remarks>
public sealed record Customer
{
    public required string Name { get; init; }

    public string? Email { get; init; }
}
