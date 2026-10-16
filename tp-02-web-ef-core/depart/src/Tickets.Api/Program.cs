using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tickets.Api.Data;
using Tickets.Api.Diagnostics;
using Tickets.Api.Endpoints;
using Tickets.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

// ---- Services: everything registered here can be injected anywhere ----

// TODO Exercise 02: register SingletonOperation, ScopedOperation and TransientOperation,
// each with the lifetime its name says.

// TODO Exercise 03: bind the "Tickets" section to TicketsOptions, validate it with its DataAnnotations,
// and make the application refuse to start when it is invalid.

// Exercise 04. TODO: add the OpenAPI document and the validation of request bodies.
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// The in-memory repository, until exercise 05.
// TODO Exercise 05: register TicketsDbContext on SQL Server with the "Tickets" connection string,
// then replace this line with EfTicketRepository, registered as scoped like the DbContext.
builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// ---- Pipeline: middlewares run in this order for every request ----

// TODO Exercise 01: add RequestTimingMiddleware at the very start of the pipeline.

// In Development, Minimal APIs throw BadHttpRequestException on an unreadable body (bad JSON, unknown enum value).
// Without a selector the exception handler would turn it into a 500; the client sent a bad request, so answer 400.
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    StatusCodeSelector = exception => exception is BadHttpRequestException badRequest
        ? badRequest.StatusCode
        : StatusCodes.Status500InternalServerError,
});
app.UseStatusCodePages();

// TODO Exercise 04: in Development only, expose the OpenAPI document and the Scalar page.

// ---- Endpoints ----

app.MapGet("/", () => "Tickets API");

// Exercise 02: each parameter marked [FromServices] is resolved separately from the container.
app.MapGet("/lifetimes", (
    [FromServices] SingletonOperation singleton1, [FromServices] SingletonOperation singleton2,
    [FromServices] ScopedOperation scoped1, [FromServices] ScopedOperation scoped2,
    [FromServices] TransientOperation transient1, [FromServices] TransientOperation transient2) => new
    {
        Singleton = new[] { singleton1.Id, singleton2.Id },
        Scoped = new[] { scoped1.Id, scoped2.Id },
        Transient = new[] { transient1.Id, transient2.Id },
    });

// Exercise 03: IOptionsSnapshot re-reads the configuration at each request.
app.MapGet("/settings", (IOptionsSnapshot<TicketsOptions> options) => options.Value);

app.MapTicketEndpoints();

app.Run();

// Lets the integration tests start the application with WebApplicationFactory<Program>.
public partial class Program;
