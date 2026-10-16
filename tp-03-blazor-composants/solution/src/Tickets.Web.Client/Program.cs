using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// Runs in the browser when a component uses the InteractiveWebAssembly render mode.
var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();
