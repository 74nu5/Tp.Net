using System.ComponentModel.DataAnnotations;
using Tickets.Api.Domain;

namespace Tickets.Api.Contracts;

// Contracts are what the API exposes. They are separate from the entities so that the database schema
// can change without breaking clients, and so that clients cannot set fields such as Id or Status.

public sealed record CreateTicketRequest
{
    [Required, StringLength(200)]
    public string Title { get; init; } = "";

    [StringLength(2000)]
    public string? Description { get; init; }

    public TicketPriority Priority { get; init; } = TicketPriority.Normal;
}

public sealed record UpdateTicketRequest
{
    [Required, StringLength(200)]
    public string Title { get; init; } = "";

    [StringLength(2000)]
    public string? Description { get; init; }

    public TicketPriority Priority { get; init; } = TicketPriority.Normal;
}

public sealed record AddCommentRequest
{
    [Required, StringLength(100)]
    public string Author { get; init; } = "";

    [Required, StringLength(2000)]
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
    DateTimeOffset? ClosedAt,
    IReadOnlyList<CommentResponse> Comments);

/// <summary>A light view of a ticket for lists: a projection, not the whole entity.</summary>
public sealed record TicketSummary(int Id, string Title, TicketStatus Status, int CommentCount);

/// <summary>TP-05: the result of POST /tickets/import.</summary>
public sealed record ImportResponse(int Imported);

public static class TicketMapping
{
    public static TicketResponse ToResponse(this Ticket ticket) => new(
        ticket.Id,
        ticket.Title,
        ticket.Description,
        ticket.Priority,
        ticket.Status,
        ticket.CreatedAt,
        ticket.ClosedAt,
        [.. ticket.Comments.OrderBy(comment => comment.Id).Select(comment => comment.ToResponse())]);

    public static CommentResponse ToResponse(this Comment comment) =>
        new(comment.Id, comment.Author, comment.Text, comment.CreatedAt);
}
