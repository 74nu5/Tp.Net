using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Tickets.Api.Contracts;
using Tickets.Api.Data;
using Tickets.Api.Domain;
using Tickets.Api.Settings;

namespace Tickets.Api.Endpoints;

/// <summary>
/// Exercises 04 and 07: the HTTP surface of the API, as Minimal API endpoints.
/// </summary>
public static class TicketEndpoints
{
    public static IEndpointRouteBuilder MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        // A route group shares the prefix and the OpenAPI tag between endpoints.
        var tickets = app.MapGroup("/tickets").WithTags("Tickets");

        tickets.MapGet("/", ListAsync);
        tickets.MapGet("/summary", SummaryAsync);

        // TODO Exercise 04: GET /tickets/{id}, POST /tickets, PUT /tickets/{id}, DELETE /tickets/{id}.
        // TODO Exercise 07: POST /tickets/{id}/comments, POST /tickets/{id}/close.

        return app;
    }

    // Parameters are bound by convention: route values and query strings by name, services from DI,
    // complex types from the JSON body. TypedResults describe the possible answers, for OpenAPI too.
    private static async Task<Ok<List<TicketResponse>>> ListAsync(
        ITicketRepository repository, IOptionsSnapshot<TicketsOptions> options, CancellationToken cancellationToken, int page = 1)
    {
        var tickets = await repository.ListAsync(Math.Max(page, 1), options.Value.PageSize, cancellationToken);
        return TypedResults.Ok(tickets.Select(t => t.ToResponse()).ToList());
    }

    private static async Task<Ok<IReadOnlyList<TicketSummary>>> SummaryAsync(ITicketRepository repository, CancellationToken cancellationToken) =>
        TypedResults.Ok(await repository.GetSummariesAsync(cancellationToken));

    // ProblemDetails (RFC 9457): the standard JSON shape for HTTP errors.
    private static ProblemHttpResult NotFound(int id) =>
        TypedResults.Problem(title: "Ticket not found", detail: $"No ticket has the id {id}.", statusCode: StatusCodes.Status404NotFound);

    private static ProblemHttpResult Conflict(string detail) =>
        TypedResults.Problem(title: "Conflict", detail: detail, statusCode: StatusCodes.Status409Conflict);
}
