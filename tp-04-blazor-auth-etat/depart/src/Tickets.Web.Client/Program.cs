using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// Runs in the browser when a component uses the InteractiveWebAssembly render mode.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// TODO TP-04, exercise 04: register FavoritesState here too, for the components rendered in the browser.

await builder.Build().RunAsync();
