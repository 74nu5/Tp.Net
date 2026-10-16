using Microsoft.EntityFrameworkCore;
using Tickets.Api.Domain;

namespace Tickets.Api.Data;

/// <summary>
/// Exercise 05: the unit of work with the database. One DbSet per table we query directly.
/// </summary>
public sealed class TicketsDbContext(DbContextOptions<TicketsDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO Exercise 05, with the Fluent API:
        // - Ticket.Title: required, at most 200 characters; Ticket.Description: at most 2000;
        // - Ticket.Priority and Ticket.Status: stored as text (HasConversion<string>()), at most 20 characters;
        // - an index on Ticket.Status;
        // - a ticket has many comments; deleting a ticket deletes its comments;
        // - Comment.Author: required, at most 100 characters; Comment.Text: required, at most 2000.
    }
}
