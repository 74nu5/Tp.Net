using Tickets.Api.Domain;

namespace Tickets.Api.Contracts;

// Contracts are what the API exposes. They are separate from the entities so that the database schema
// can change without breaking clients, and so that clients cannot set fields such as Id or Status.

// TODO Exercise 04: validate the requests. A title is required and at most 200 characters long;
// a description is at most 2000 characters long. Same for UpdateTicketRequest.
public sealed record CreateTicketRequest
{
    public string Title { get; init; } = "";

    public string? Description { get; init; }

    public TicketPriority Priority { get; init; } = TicketPriority.Normal;
}

public sealed record UpdateTicketRequest
{
    public string Title { get; init; } = "";

    public string? Description { get; init; }

    public TicketPriority Priority { get; init; } = TicketPriority.Normal;
}

// TODO Exercise 07: an author (at most 100 characters) and a text (at most 2000) are required.
public sealed record AddCommentRequest
{
    public string Author { get; init; } = "";

    public string Text { get; init; } = "";
}

public sealed record CommentResponse(int Id, string Author, string Text, DateTimeOffset CreatedAt);

public sealed record TicketResponse(
    int Id,
    string Title,
    string? Description,
    TicketPriority Priority,
    TicketStatus Status,
    DateTimeOffset CreatedAt,
    IReadOnlyList<CommentResponse> Comments);

/// <summary>A light view of a ticket for lists: a projection, not the whole entity.</summary>
public sealed record TicketSummary(int Id, string Title, TicketStatus Status, int CommentCount);

public static class TicketMapping
{
    public static TicketResponse ToResponse(this Ticket ticket) => new(
        ticket.Id,
        ticket.Title,
        ticket.Description,
        ticket.Priority,
        ticket.Status,
        ticket.CreatedAt,
        [.. ticket.Comments.OrderBy(comment => comment.Id).Select(comment => comment.ToResponse())]);

    public static CommentResponse ToResponse(this Comment comment) =>
        new(comment.Id, comment.Author, comment.Text, comment.CreatedAt);
}
