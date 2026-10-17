namespace Tickets.Web.Client.State;

/// <summary>
/// TP-04, exercise 04: state shared by several components that do not know each other.
/// TODO Exercise 04: keep the ids of the favorite tickets, and raise Changed after every change.
/// Then register it as scoped in both Program.cs files (Tickets.Web and Tickets.Web.Client).
/// </summary>
public sealed class FavoritesState
{
    /// <summary>Raised after every change, so that the components that display the state can re-render.</summary>
    public event Action? Changed;

    public int Count => throw new NotImplementedException("TODO Exercise 04");

    public bool IsFavorite(int ticketId) => throw new NotImplementedException("TODO Exercise 04");

    public void Toggle(int ticketId)
    {
        // TODO Exercise 04: change the state, then raise Changed. Replace both lines: this Invoke is only here
        // so that the starting code compiles without warning, and it comes before the change.
        Changed?.Invoke();
        throw new NotImplementedException("TODO Exercise 04");
    }
}
