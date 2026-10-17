using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Tickets.Api.Data;
using Tickets.Api.Diagnostics;
using Tickets.Api.Endpoints;
using Tickets.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

// TODO Exercise 04: add the Aspire service defaults (Tickets.ServiceDefaults, already referenced).

// ---- Services: everything registered here can be injected anywhere ----

// TP-02, exercise 02: the same kind of class, three lifetimes.
builder.Services.AddSingleton<SingletonOperation>();
builder.Services.AddScoped<ScopedOperation>();
builder.Services.AddTransient<TransientOperation>();

// TP-02, exercise 03: bind the "Tickets" section, validate it with the DataAnnotations, and fail at startup if it is wrong
// rather than at the first request that reads it.
builder.Services.AddOptions<TicketsOptions>()
    .BindConfiguration(TicketsOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// TP-02, exercise 04: OpenAPI document, ProblemDetails for errors, validation of request bodies, enums as text in JSON.
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// TP-02, exercise 05: EF Core on SQL Server. The connection string comes from user-secrets in development,
// from an environment variable on a server: never from appsettings.json.
// It is read when a DbContext is created, not here: the tests replace it after Program.cs has run.
builder.Services.AddDbContext<TicketsDbContext>((services, options) =>
    options.UseSqlServer(services.GetRequiredService<IConfiguration>().GetConnectionString("Tickets")
        ?? throw new InvalidOperationException(
            "Connection string 'Tickets' is missing. Set it with: dotnet user-secrets set \"ConnectionStrings:Tickets\" \"<connection string>\"")));
// TP-05: the endpoints receive a CachedTicketRepository, which wraps the EF Core repository (a decorator).
builder.Services.AddMemoryCache();
builder.Services.AddScoped<EfTicketRepository>();
builder.Services.AddScoped<ITicketRepository>(services =>
    new CachedTicketRepository(services.GetRequiredService<EfTicketRepository>(), services.GetRequiredService<IMemoryCache>()));
builder.Services.AddSingleton(TimeProvider.System);

// TP-04, exercise 01: JWT bearer authentication. With no argument, AddJwtBearer reads the
// "Authentication:Schemes:Bearer" configuration section, which `dotnet user-jwts` fills in for development.
builder.Services.AddAuthentication().AddJwtBearer();

// TP-04, exercise 02: authorization policies. Authentication says who you are; authorization what you may do.
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(TicketPolicies.Admin, policy => policy.RequireRole("admin"));

var app = builder.Build();

// ---- Pipeline: middlewares run in this order for every request ----

app.UseMiddleware<RequestTimingMiddleware>();   // TP-02, exercise 01
// In Development, Minimal APIs throw BadHttpRequestException on an unreadable body (bad JSON, unknown enum value).
// Without a selector the exception handler would turn it into a 500; the client sent a bad request, so answer 400.
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    StatusCodeSelector = exception => exception is BadHttpRequestException badRequest
        ? badRequest.StatusCode
        : StatusCodes.Status500InternalServerError,
});
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                 // /openapi/v1.json
    app.MapScalarApiReference();      // /scalar: an interactive page to call the API
}

// ---- Endpoints ----

// TODO Exercise 04: map /health and /alive.

app.MapGet("/", () => "Tickets API");

// TP-04, exercise 01: who is calling? ClaimsPrincipal is bound from HttpContext.User.
app.MapGet("/me", (ClaimsPrincipal user) => new
{
    Name = user.Identity?.Name,
    Roles = user.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray(),
    IsAdmin = user.IsInRole("admin"),
}).RequireAuthorization();

// TP-02, exercise 02: each parameter marked [FromServices] is resolved separately from the container.
app.MapGet("/lifetimes", (
    [FromServices] SingletonOperation singleton1, [FromServices] SingletonOperation singleton2,
    [FromServices] ScopedOperation scoped1, [FromServices] ScopedOperation scoped2,
    [FromServices] TransientOperation transient1, [FromServices] TransientOperation transient2) => new
    {
        Singleton = new[] { singleton1.Id, singleton2.Id },
        Scoped = new[] { scoped1.Id, scoped2.Id },
        Transient = new[] { transient1.Id, transient2.Id },
    });

// TP-02, exercise 03: IOptionsSnapshot re-reads the configuration at each request.
app.MapGet("/settings", (IOptionsSnapshot<TicketsOptions> options) => options.Value);

app.MapTicketEndpoints();

app.Run();

// Lets the integration tests start the application with WebApplicationFactory<Program>.
public partial class Program;
