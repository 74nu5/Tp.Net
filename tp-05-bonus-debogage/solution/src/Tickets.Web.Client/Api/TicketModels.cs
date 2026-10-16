using System.ComponentModel.DataAnnotations;

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
/// The DataAnnotations are the same rules as on the API: the user sees the errors before sending.
/// </summary>
public sealed class NewTicketForm
{
    [Required(ErrorMessage = "The title is required.")]
    [StringLength(200, ErrorMessage = "The title must be at most 200 characters long.")]
    public string Title { get; set; } = "";

    [StringLength(2000)]
    public string? Description { get; set; }

    public TicketPriority Priority { get; set; } = TicketPriority.Normal;
}
