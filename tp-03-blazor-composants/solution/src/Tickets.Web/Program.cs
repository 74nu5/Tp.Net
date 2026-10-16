using Tickets.Web.Client.Api;
using Tickets.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Razor components, with the two interactive render modes available: Server (SignalR) and WebAssembly.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// A typed HttpClient for the Tickets API (TP-02), used by the pages rendered on the server.
builder.Services.AddHttpClient<ITicketsClient, TicketsClient>((services, client) =>
    client.BaseAddress = new Uri(services.GetRequiredService<IConfiguration>()["TicketsApi:BaseUrl"] ?? "http://localhost:5080/"));

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Tickets.Web.Client._Imports).Assembly);

app.Run();

// Lets the tests start the application with WebApplicationFactory<Program>.
public partial class Program;
