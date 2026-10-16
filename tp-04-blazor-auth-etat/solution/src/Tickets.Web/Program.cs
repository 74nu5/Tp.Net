using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Tickets.Web.Client.Api;
using Tickets.Web.Client.State;
using Tickets.Web.Components;
using Tickets.Web.Security;

var builder = WebApplication.CreateBuilder(args);

// Razor components, with the two interactive render modes available: Server (SignalR) and WebAssembly.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// TP-04, exercise 03: who is the user? A cookie, written by the login page.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/account/login");
builder.Services.AddAuthorization();

// Makes the AuthenticationState available to every component, in every render mode, as a cascading value.
builder.Services.AddCascadingAuthenticationState();

// A typed HttpClient for the Tickets API (TP-02), used by the pages rendered on the server.
// TP-04: ApiTokenHandler adds the bearer token the API now requires for writes.
builder.Services.AddTransient<ApiTokenHandler>();
builder.Services.AddHttpClient<ITicketsClient, TicketsClient>((services, client) =>
        client.BaseAddress = new Uri(services.GetRequiredService<IConfiguration>()["TicketsApi:BaseUrl"] ?? "http://localhost:5080/"))
    .AddHttpMessageHandler<ApiTokenHandler>();

// TP-04, exercise 04: one FavoritesState per circuit, shared by every component of the page.
builder.Services.AddScoped<FavoritesState>();

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

// TP-04, exercise 03: logging out is a plain HTTP endpoint that deletes the cookie.
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
