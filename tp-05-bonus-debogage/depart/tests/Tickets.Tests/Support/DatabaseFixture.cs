using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Api.Data;

namespace Tickets.Tests.Support;

/// <summary>
/// Recreates the test database once, before the tests of the "Database" collection run.
/// Does nothing until exercise 05 is done (no DbContext registered, or no migration yet).
/// </summary>
public sealed class DatabaseFixture : IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await using var factory = new TicketsApiFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetService<TicketsDbContext>();
        if (db is null || !db.Database.GetMigrations().Any())
        {
            return;
        }

        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

/// <summary>Tests that use the database run one after the other, after <see cref="DatabaseFixture"/>.</summary>
[CollectionDefinition(Name)]
public sealed class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Database";
}
