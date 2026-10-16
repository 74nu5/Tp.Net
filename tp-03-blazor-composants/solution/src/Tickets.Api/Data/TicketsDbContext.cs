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
        // Fluent API: the mapping lives here rather than in attributes on the entities.
        modelBuilder.Entity<Ticket>(ticket =>
        {
            ticket.Property(t => t.Title).HasMaxLength(200).IsRequired();
            ticket.Property(t => t.Description).HasMaxLength(2000);

            // Store enums as readable text ("High") rather than numbers (2): safer when values are reordered.
            ticket.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
            ticket.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            ticket.HasIndex(t => t.Status);

            ticket.HasMany(t => t.Comments)
                .WithOne(c => c.Ticket)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(comment =>
        {
            comment.Property(c => c.Author).HasMaxLength(100).IsRequired();
            comment.Property(c => c.Text).HasMaxLength(2000).IsRequired();
        });
    }
}
