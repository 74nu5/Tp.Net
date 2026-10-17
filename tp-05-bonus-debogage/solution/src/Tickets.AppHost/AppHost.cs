// TP-05, exercise 04: the application model. Each AddProject is a resource that Aspire starts,
// watches and shows in its dashboard. Projects.Tickets_Api is generated from the ProjectReference.
var builder = DistributedApplication.CreateBuilder(args);

// The API keeps its own configuration (user-secrets for the connection string and the JWT signing key).
// Aspire polls its /health endpoint (MapDefaultEndpoints) and shows the result in the dashboard.
var api = builder.AddProject<Projects.Tickets_Api>("api")
    .WithHttpHealthCheck("/health");

// The front end gets the address of the API from Aspire rather than from its appsettings.json,
// and starts only once the API is running and healthy.
builder.AddProject<Projects.Tickets_Web>("web")
    .WithEnvironment("TicketsApi__BaseUrl", api.GetEndpoint("http"))
    .WaitFor(api);

builder.Build().Run();
