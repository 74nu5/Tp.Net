namespace Tickets.Web.Client.State;

/// <summary>
/// TP-04, exercise 04: state shared by several components that do not know each other.
/// Registered as scoped: with Interactive Server, one instance per circuit, so per user and per browser tab.
/// </summary>
public sealed class FavoritesState
{
    private readonly HashSet<int> _ticketIds = [];

    /// <summary>Raised after every change, so that the components that display the state can re-render.</summary>
    public event Action? Changed;

    public int Count => _ticketIds.Count;

    public bool IsFavorite(int ticketId) => _ticketIds.Contains(ticketId);

    public void Toggle(int ticketId)
    {
        if (!_ticketIds.Remove(ticketId))
        {
            _ticketIds.Add(ticketId);
        }

        Changed?.Invoke();
    }
}
