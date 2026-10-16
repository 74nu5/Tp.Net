using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Api.Data;
using Tickets.Api.Domain;
using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise05;

[Collection(DatabaseCollection.Name)]
public class EntityFrameworkTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private CancellationToken Token => TestContext.Current.CancellationToken;

    private TicketsDbContext Db(IServiceScope scope) =>
        scope.ServiceProvider.GetService<TicketsDbContext>()
        ?? throw new InvalidOperationException("TicketsDbContext is not registered: call AddDbContext in Program.cs.");

    [Fact]
    public void Model_LimitsTheTitle_To200Characters()
    {
        using var scope = factory.Services.CreateScope();
        var title = Db(scope).Model.FindEntityType(typeof(Ticket))!.FindProperty(nameof(Ticket.Title))!;

        Assert.Equal(200, title.GetMaxLength());
        Assert.False(title.IsNullable);
    }

    [Fact]
    public void Model_StoresEnumsAsText()
    {
        using var scope = factory.Services.CreateScope();
        var priority = Db(scope).Model.FindEntityType(typeof(Ticket))!.FindProperty(nameof(Ticket.Priority))!;

        Assert.Equal(typeof(string), priority.GetTypeMapping().Converter?.ProviderClrType);
    }

    [Fact]
    public void Model_DeletesTheComments_WithTheirTicket()
    {
        using var scope = factory.Services.CreateScope();
        var foreignKey = Db(scope).Model.FindEntityType(typeof(Comment))!.GetForeignKeys().Single();

        Assert.Equal(typeof(Ticket), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Fact]
    public async Task Database_IsCreated_ByTheMigrations()
    {
        using var scope = factory.Services.CreateScope();

        var applied = await Db(scope).Database.GetAppliedMigrationsAsync(Token);

        Assert.Contains(applied, migration => migration.EndsWith("_InitialCreate", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Tickets_SurviveARestartOfTheApplication()
    {
        var title = HttpExtensions.UniqueTitle("Persistent");
        int id;
        await using (var first = new TicketsApiFactory())
        {
            id = (await first.CreateClient().CreateTicketAsync(title))["id"]!.GetValue<int>();
        }

        await using var second = new TicketsApiFactory();
        var response = await second.CreateClient().GetAsync($"/tickets/{id}", Token);

        Assert.True(response.IsSuccessStatusCode, "The ticket disappeared with the application: is the API still using the in-memory repository?");
        Assert.Equal(title, (await response.ReadJsonAsync())["title"]!.GetValue<string>());
    }
}
