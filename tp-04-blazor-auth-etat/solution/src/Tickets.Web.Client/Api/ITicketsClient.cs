namespace Tickets.Web.Client.Api;

/// <summary>
/// What the pages need from the Tickets API. An interface, so that the component tests can replace it with a fake.
/// </summary>
public interface ITicketsClient
{
    Task<IReadOnlyList<TicketDto>> GetTicketsAsync(CancellationToken cancellationToken = default);

    Task<TicketDto> CreateTicketAsync(NewTicketForm ticket, CancellationToken cancellationToken = default);
}
