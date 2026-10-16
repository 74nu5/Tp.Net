using Tickets.Web.Client.Components;
using Tickets.Web.Client.State;
using Tickets.Web.Tests.Support;

namespace Tickets.Web.Tests.Exercise04;

public class FavoritesStateTests
{
    [Fact]
    public void Toggle_AddsThenRemoves()
    {
        var favorites = new FavoritesState();

        favorites.Toggle(7);
        Assert.True(favorites.IsFavorite(7));
        Assert.Equal(1, favorites.Count);

        favorites.Toggle(7);
        Assert.False(favorites.IsFavorite(7));
        Assert.Equal(0, favorites.Count);
    }

    [Fact]
    public void Toggle_RaisesChanged()
    {
        var favorites = new FavoritesState();
        var changes = 0;
        favorites.Changed += () => changes++;

        favorites.Toggle(1);
        favorites.Toggle(2);

        Assert.Equal(2, changes);
    }
}

public class SharedStateTests : ComponentTest
{
    private static readonly Client.Api.TicketDto Printer = FakeTicketsClient.Ticket(7, "Printer on fire", description: "Third floor");

    [Fact]
    public void FavoriteButton_OnACard_UpdatesTheSharedState()
    {
        var card = Render<TicketCard>(parameters => parameters.Named("Ticket", Printer));

        card.Find("button.favorite").Click();

        Assert.True(Favorites.IsFavorite(7));
    }

    [Fact]
    public void Counter_FollowsChangesMadeElsewhere()
    {
        var counter = Render<FavoritesCounter>();

        Favorites.Toggle(1);
        Favorites.Toggle(2);

        counter.WaitForAssertion(() => Assert.Contains("2", counter.Find(".favorites-count").TextContent));
    }

    [Fact]
    public void CascadingCompact_HidesTheDescription()
    {
        var normal = Render<TicketCard>(parameters => parameters.Named("Ticket", Printer));
        var compact = Render<TicketCard>(parameters => parameters.Named("Ticket", Printer).AddCascadingValue("Compact", true));

        Assert.Contains("Third floor", normal.Markup);
        Assert.DoesNotContain("Third floor", compact.Markup);
    }
}
