namespace Tickets.Api.Diagnostics;

/// <summary>
/// Exercise 02: three identical classes, registered with three different lifetimes.
/// Each instance gets a new id, so the ids show when the container creates a new instance.
/// </summary>
public abstract class Operation
{
    public Guid Id { get; } = Guid.NewGuid();
}

/// <summary>One instance for the whole application.</summary>
public sealed class SingletonOperation : Operation;

/// <summary>One instance per HTTP request.</summary>
public sealed class ScopedOperation : Operation;

/// <summary>A new instance every time it is asked for.</summary>
public sealed class TransientOperation : Operation;
