namespace Tickets.Web.Client.State;

/// <summary>
/// TP-04, exercise 04: state shared by several components that do not know each other.
/// TODO: keep the ids of the favorite tickets, and raise Changed after every change.
/// Then register it as scoped in both Program.cs files (Tickets.Web and Tickets.Web.Client).
/// </summary>
public sealed class FavoritesState
{
    /// <summary>Raised after every change, so that the components that display the state can re-render.</summary>
    public event Action? Changed;

    public int Count => throw new NotImplementedException("TODO TP-04 exercise 04");

    public bool IsFavorite(int ticketId) => throw new NotImplementedException("TODO TP-04 exercise 04");

    public void Toggle(int ticketId)
    {
        Changed?.Invoke();
        throw new NotImplementedException("TODO TP-04 exercise 04");
    }
}
