using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Tickets.Api.Contracts;
using Tickets.Api.Data;
using Tickets.Api.Domain;
using Tickets.Api.Settings;

namespace Tickets.Api.Endpoints;

/// <summary>
/// TP-02, exercises 04 and 07: the HTTP surface of the API, as Minimal API endpoints.
/// </summary>
public static class TicketEndpoints
{
    public static IEndpointRouteBuilder MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        // A route group shares the prefix and the OpenAPI tag between endpoints.
        var tickets = app.MapGroup("/tickets").WithTags("Tickets");

        tickets.MapGet("/", ListAsync);
        tickets.MapGet("/summary", SummaryAsync);
        tickets.MapGet("/{id:int}", GetAsync).WithName("GetTicket");
        // TODO Exercise 02: reading stays open; writing needs an authenticated caller; deleting needs the admin policy.
        tickets.MapPost("/", CreateAsync);
        tickets.MapPut("/{id:int}", UpdateAsync);
        tickets.MapDelete("/{id:int}", DeleteAsync);
        tickets.MapPost("/{id:int}/comments", AddCommentAsync);
        tickets.MapPost("/{id:int}/close", CloseAsync);

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

    private static async Task<Results<Ok<TicketResponse>, ProblemHttpResult>> GetAsync(
        int id, ITicketRepository repository, CancellationToken cancellationToken) =>
        await repository.GetAsync(id, cancellationToken) is { } ticket
            ? TypedResults.Ok(ticket.ToResponse())
            : NotFound(id);

    // The request body is validated before this method runs (AddValidation in Program.cs):
    // an invalid body never gets here and the client receives a 400 ValidationProblem.
    private static async Task<CreatedAtRoute<TicketResponse>> CreateAsync(
        CreateTicketRequest request, ITicketRepository repository, CancellationToken cancellationToken)
    {
        var ticket = await repository.CreateAsync(
            new Ticket { Title = request.Title, Description = request.Description, Priority = request.Priority },
            cancellationToken);
        return TypedResults.CreatedAtRoute(ticket.ToResponse(), "GetTicket", new { id = ticket.Id });
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> UpdateAsync(
        int id, UpdateTicketRequest request, ITicketRepository repository, CancellationToken cancellationToken) =>
        await repository.UpdateAsync(id, request.Title, request.Description, request.Priority, cancellationToken)
            ? TypedResults.NoContent()
            : NotFound(id);

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
        int id, ITicketRepository repository, CancellationToken cancellationToken) =>
        await repository.DeleteAsync(id, cancellationToken) ? TypedResults.NoContent() : NotFound(id);

    private static async Task<Results<Created<CommentResponse>, ProblemHttpResult>> AddCommentAsync(
        int id, AddCommentRequest request, ITicketRepository repository, CancellationToken cancellationToken)
    {
        var (result, comment) = await repository.AddCommentAsync(id, request.Author, request.Text, cancellationToken);
        return result switch
        {
            TicketChange.Applied => TypedResults.Created($"/tickets/{id}", comment!.ToResponse()),
            TicketChange.AlreadyClosed => Conflict($"Ticket {id} is closed."),
            _ => NotFound(id),
        };
    }

    private static async Task<Results<Ok<TicketResponse>, ProblemHttpResult>> CloseAsync(
        int id, ITicketRepository repository, CancellationToken cancellationToken)
    {
        var (result, ticket) = await repository.CloseAsync(id, cancellationToken);
        return result switch
        {
            TicketChange.Applied => TypedResults.Ok(ticket!.ToResponse()),
            TicketChange.AlreadyClosed => Conflict($"Ticket {id} is already closed."),
            _ => NotFound(id),
        };
    }

    // ProblemDetails (RFC 9457): the standard JSON shape for HTTP errors.
    private static ProblemHttpResult NotFound(int id) =>
        TypedResults.Problem(title: "Ticket not found", detail: $"No ticket has the id {id}.", statusCode: StatusCodes.Status404NotFound);

    private static ProblemHttpResult Conflict(string detail) =>
        TypedResults.Problem(title: "Conflict", detail: detail, statusCode: StatusCodes.Status409Conflict);
}
