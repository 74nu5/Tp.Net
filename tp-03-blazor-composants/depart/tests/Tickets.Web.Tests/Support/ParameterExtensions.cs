using Microsoft.AspNetCore.Components;

namespace Tickets.Web.Tests.Support;

public static class ParameterExtensions
{
    /// <summary>
    /// Passes a parameter by name, so that a test compiles before the component declares the parameter,
    /// and fails with a clear message while it does not.
    /// </summary>
    public static ComponentParameterCollectionBuilder<TComponent> Named<TComponent, TValue>(
        this ComponentParameterCollectionBuilder<TComponent> parameters, string name, TValue value)
        where TComponent : IComponent
    {
        if (!parameters.TryAdd(name, value))
        {
            Assert.Fail($"{typeof(TComponent).Name} has no [Parameter] named {name} of type {typeof(TValue).Name}.");
        }

        return parameters;
    }
}
