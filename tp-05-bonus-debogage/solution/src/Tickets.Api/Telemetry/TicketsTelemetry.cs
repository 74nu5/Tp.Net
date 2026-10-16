using System.Diagnostics;

namespace Tickets.Api.Telemetry;

/// <summary>
/// TP-05, exercise 04: our own spans. In .NET, a span is an <see cref="Activity"/>, created by an <see cref="ActivitySource"/>.
/// </summary>
/// <remarks>
/// The service defaults listen to the source named after the application (builder.Environment.ApplicationName),
/// which is "Tickets.Api": the spans of this source go to the Aspire dashboard with no extra configuration.
/// StartActivity returns null when nobody listens, hence the ?. on every use.
/// </remarks>
public static class TicketsTelemetry
{
    public const string SourceName = "Tickets.Api";

    public static readonly ActivitySource Source = new(SourceName);
}
