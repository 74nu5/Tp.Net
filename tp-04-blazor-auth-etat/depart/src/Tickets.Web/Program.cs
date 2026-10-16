using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Tickets.Web.Client.Api;
using Tickets.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Razor components, with the two interactive render modes available: Server (SignalR) and WebAssembly.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// TODO TP-04, exercise 03: cookie authentication (login page: /account/login), authorization,
// and the cascading authentication state for the components.

// A typed HttpClient for the Tickets API (TP-02), used by the pages rendered on the server.
// TODO TP-04, exercise 03: add ApiTokenHandler to its pipeline (AddTransient, then AddHttpMessageHandler).
builder.Services.AddHttpClient<ITicketsClient, TicketsClient>((services, client) =>
    client.BaseAddress = new Uri(services.GetRequiredService<IConfiguration>()["TicketsApi:BaseUrl"] ?? "http://localhost:5080/"));

// TODO TP-04, exercise 04: register FavoritesState, one instance per circuit.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();

// Provided. Logging out is a plain HTTP endpoint that deletes the cookie.
app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return TypedResults.LocalRedirect("/");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Tickets.Web.Client._Imports).Assembly);

app.Run();

// Lets the tests start the application with WebApplicationFactory<Program>.
public partial class Program;
