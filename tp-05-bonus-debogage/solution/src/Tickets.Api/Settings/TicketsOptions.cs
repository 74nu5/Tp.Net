using System.ComponentModel.DataAnnotations;

namespace Tickets.Api.Settings;

/// <summary>
/// TP-02, exercise 03: the "Tickets" section of the configuration, bound to a class.
/// </summary>
public sealed class TicketsOptions
{
    public const string SectionName = "Tickets";

    /// <summary>Number of tickets per page in GET /tickets.</summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    /// <summary>Address shown to users who need help.</summary>
    [Required, EmailAddress]
    public string SupportEmail { get; set; } = "";
}
