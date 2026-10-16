using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tickets.Web.Client.State;

// Runs in the browser when a component uses the InteractiveWebAssembly render mode.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// TP-04, exercise 04: in the browser there is a single user, so scoped and singleton behave alike.
builder.Services.AddScoped<FavoritesState>();

await builder.Build().RunAsync();
