namespace Tickets.Web.Security;

/// <summary>
/// TP-04, exercise 03: adds the API token to every request of the typed HttpClient.
/// A DelegatingHandler sits in the HttpClient pipeline, like a middleware on the client side.
/// TODO: when the configuration has a "TicketsApi:Token" value, add the header
/// "Authorization: Bearer &lt;token&gt;" to the request before passing it on.
/// </summary>
public sealed class ApiTokenHandler(IConfiguration configuration) : DelegatingHandler
{
    private readonly IConfiguration _configuration = configuration;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        base.SendAsync(request, cancellationToken);
}
