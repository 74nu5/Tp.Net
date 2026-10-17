namespace Tickets.Api.Settings;

/// <summary>
/// Exercise 03: the "Tickets" section of the configuration, bound to a class.
/// TODO Exercise 03: PageSize must be between 1 and 100; SupportEmail is required and must be an email address.
/// </summary>
public sealed class TicketsOptions
{
    public const string SectionName = "Tickets";

    /// <summary>Number of tickets per page in GET /tickets.</summary>
    public int PageSize { get; set; } = 20;

    /// <summary>Address shown to users who need help.</summary>
    public string SupportEmail { get; set; } = "";
}
