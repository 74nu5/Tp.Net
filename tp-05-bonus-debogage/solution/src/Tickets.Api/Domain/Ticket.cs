namespace Tickets.Api.Domain;

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

/// <summary>A support ticket. An EF Core entity: a plain class, mapped to a table by TicketsDbContext.</summary>
public sealed class Ticket
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string? Description { get; set; }

    public TicketPriority Priority { get; set; } = TicketPriority.Normal;

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>TP-02, exercise 07: added by the second migration.</summary>
    public DateTimeOffset? ClosedAt { get; set; }

    public List<Comment> Comments { get; set; } = [];

    /// <summary>A shortcut for the pages: the Comments collection must be loaded (Include).</summary>
    /// <remarks>A read-only property is not mapped to a column: EF Core cannot use it inside a query.</remarks>
    public int CommentCount => Comments.Count;
}

/// <summary>A comment on a ticket: the "many" side of a one-to-many relationship.</summary>
public sealed class Comment
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public Ticket? Ticket { get; set; }

    public string Author { get; set; } = "";

    public string Text { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; }
}
