using Tickets.Api.Tests.Support;

namespace Tickets.Api.Tests.Exercise08;

/// <summary>
/// Exercise 08: write at least two integration tests of your own here. They go through the whole application:
/// routing, validation, the repository and SQL Server. Look at the other test files for examples:
/// factory.CreateClient(), PostAsJsonAsync, ReadJsonAsync, HttpExtensions.UniqueTitle.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class MyApiTests(TicketsApiFactory factory) : IClassFixture<TicketsApiFactory>
{
    private readonly TicketsApiFactory _factory = factory;
}
