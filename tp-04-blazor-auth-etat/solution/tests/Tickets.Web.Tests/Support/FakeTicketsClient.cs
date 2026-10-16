using Tickets.Web.Client.Api;

namespace Tickets.Web.Tests.Support;

/// <summary>Replaces the real API in the component tests: no network, and full control over the answers.</summary>
public sealed class FakeTicketsClient : ITicketsClient
{
    public List<TicketDto> Tickets { get; } = [];

    public List<NewTicketForm> Created { get; } = [];

    /// <summary>When set, GetTicketsAsync throws it.</summary>
    public Exception? Failure { get; set; }

    /// <summary>When set, GetTicketsAsync waits for it: lets a test look at the loading state.</summary>
    public TaskCompletionSource? Gate { get; set; }

    public async Task<IReadOnlyList<TicketDto>> GetTicketsAsync(CancellationToken cancellationToken = default)
    {
        if (Gate is not null)
        {
            await Gate.Task;
        }

        return Failure is null ? Tickets : throw Failure;
    }

    public Task<TicketDto> CreateTicketAsync(NewTicketForm ticket, CancellationToken cancellationToken = default)
    {
        Created.Add(new NewTicketForm { Title = ticket.Title, Description = ticket.Description, Priority = ticket.Priority });
        return Task.FromResult(Ticket(Created.Count, ticket.Title, ticket.Priority));
    }

    public static TicketDto Ticket(int id, string title, TicketPriority priority = TicketPriority.Normal, string? description = null) =>
        new(id, title, description, priority, TicketStatus.Open, new DateTimeOffset(2026, 10, 21, 9, 0, 0, TimeSpan.Zero), null);
}
