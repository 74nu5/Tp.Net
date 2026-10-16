// TP-05, exercise 04: the application model. Each AddProject is a resource that Aspire starts,
// watches and shows in its dashboard. Projects.Tickets_Api is generated from the ProjectReference.
var builder = DistributedApplication.CreateBuilder(args);

// TODO exercise 04:
// - add the API project as a resource named "api";
// - add the front end as a resource named "web", which receives the address of the API
//   in the TicketsApi__BaseUrl environment variable and waits for the API before starting.

builder.Build().Run();
