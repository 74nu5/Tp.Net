namespace Tickets.Web.Client.Api;

// The front end describes the JSON of the API with its own types: it does not reference the API project.
// The two sides only share the HTTP contract, as with any other client.

public enum TicketPriority
{
    Low,
    Normal,
    High,
}

public enum TicketStatus
{
    Open,
    Closed,
}

public sealed record TicketDto(
    int Id,
    string Title,
    string? Description,
    TicketPriority Priority,
    TicketStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ClosedAt);

/// <summary>
/// The model of the creation form. A mutable class: EditForm writes into it as the user types.
/// TODO Exercise 05: the same rules as on the API. The title is required and at most 200 characters long,
/// the description at most 2000. Give each rule an error message for the user.
/// </summary>
public sealed class NewTicketForm
{
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    public TicketPriority Priority { get; set; } = TicketPriority.Normal;
}
