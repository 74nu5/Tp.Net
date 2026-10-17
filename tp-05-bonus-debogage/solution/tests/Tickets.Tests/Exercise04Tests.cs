extern alias AppHost;

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using Tickets.Tests.Support;

namespace Tickets.Tests.Exercise04;

public class ServiceDefaultsTests : IClassFixture<TicketsApiFactory>
{
    private readonly TicketsApiFactory _factory;

    public ServiceDefaultsTests(TicketsApiFactory factory) => _factory = factory;

    [Theory]
    [InlineData("/health")]
    [InlineData("/alive")]
    public async Task TheApi_AnswersTheHealthChecks(string path)
    {
        var response = await _factory.CreateClient().GetAsync(path, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void TheApi_RecordsTraces() =>
        Assert.NotNull(_factory.Services.GetService<TracerProvider>());
}

[Collection(DatabaseCollection.Name)]
public class CustomSpanTests : IClassFixture<TicketsApiFactory>
{
    private readonly TicketsApiFactory _factory;

    public CustomSpanTests(TicketsApiFactory factory) => _factory = factory;

    [Fact]
    public async Task TheImport_IsASpanOfItsOwn()
    {
        var spans = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tickets.Api",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => { lock (spans) { spans.Add(activity); } },
        };
        ActivitySource.AddActivityListener(listener);

        var csv = $"title,priority,description\n{HttpExtensions.UniqueTitle("Imported")},High,\n{HttpExtensions.UniqueTitle("Imported")},Low,\n";
        using var content = new StringContent(csv, new MediaTypeHeaderValue("text/csv"));
        var response = await _factory.CreateClient().PostAsync("/tickets/import", content, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
        Activity span;
        lock (spans)
        {
            span = Assert.Single(spans, activity => activity.OperationName == "ImportTickets");
        }

        Assert.Equal(2, span.GetTagItem("tickets.count"));
    }
}

public class AppHostTests
{
    private static async Task<IDistributedApplicationTestingBuilder> CreateAppHostAsync() =>
        await DistributedApplicationTestingBuilder.CreateAsync<AppHost::Projects.Tickets_AppHost>(TestContext.Current.CancellationToken);

    private static ProjectResource Project(IDistributedApplicationTestingBuilder appHost, string name) =>
        Assert.Single(appHost.Resources.OfType<ProjectResource>(), resource => resource.Name == name);

    [Fact]
    public async Task TheAppHost_StartsTheApiAndTheFrontEnd()
    {
        await using var appHost = await CreateAppHostAsync();

        Assert.Equal(["api", "web"], appHost.Resources.OfType<ProjectResource>().Select(resource => resource.Name).Order());
    }

    [Fact]
    public async Task TheApi_IsHealthChecked()
    {
        await using var appHost = await CreateAppHostAsync();

        Assert.True(Project(appHost, "api").TryGetAnnotationsOfType<HealthCheckAnnotation>(out var checks) && checks.Any(),
            "Add WithHttpHealthCheck(\"/health\") to the api resource.");
    }

    [Fact]
    public async Task TheFrontEnd_WaitsForTheApi()
    {
        await using var appHost = await CreateAppHostAsync();
        var api = Project(appHost, "api");
        var web = Project(appHost, "web");

        Assert.True(web.TryGetAnnotationsOfType<WaitAnnotation>(out var waits) && waits.Any(wait => wait.Resource == api));
    }

    [Fact]
    public async Task TheFrontEnd_GetsTheAddressOfTheApi()
    {
        await using var appHost = await CreateAppHostAsync();
        var web = Project(appHost, "web");

        // Runs the WithEnvironment callbacks as Aspire does before starting the project, without starting anything.
        var environment = new Dictionary<string, object>();
        var context = new EnvironmentCallbackContext(
            new DistributedApplicationExecutionContext(DistributedApplicationOperation.Publish), web, environment, TestContext.Current.CancellationToken);
        foreach (var annotation in web.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            await annotation.Callback(context);
        }

        Assert.True(environment.ContainsKey("TicketsApi__BaseUrl"), $"Variables given to web: {string.Join(", ", environment.Keys)}");
    }
}
