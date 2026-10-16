using Microsoft.Extensions.DependencyInjection;
using Tickets.Web.Client.Api;
using Tickets.Web.Client.State;

namespace Tickets.Web.Tests.Support;

/// <summary>
/// Base class of the component tests. bUnit renders the components in memory, without a browser:
/// we read the produced HTML and trigger events (click, change, submit) on it.
/// </summary>
public abstract class ComponentTest : BunitContext
{
    protected ComponentTest()
    {
        Services.AddSingleton<ITicketsClient>(Api);
        Services.AddSingleton(Favorites);

        // Loose: JavaScript calls nobody set up return default values instead of failing (QuickGrid uses some).
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected FakeTicketsClient Api { get; } = new();

    protected FavoritesState Favorites { get; } = new();
}
