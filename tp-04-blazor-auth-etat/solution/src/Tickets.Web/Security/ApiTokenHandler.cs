using System.Net.Http.Headers;

namespace Tickets.Web.Security;

/// <summary>
/// TP-04, exercise 03: adds the API token to every request of the typed HttpClient.
/// A DelegatingHandler sits in the HttpClient pipeline, like a middleware on the client side.
/// </summary>
/// <remarks>
/// The token is a service account token read from configuration (user-secrets in development).
/// With OpenID Connect, we would forward the access token of the logged-in user instead.
/// </remarks>
public sealed class ApiTokenHandler(IConfiguration configuration) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (configuration["TicketsApi:Token"] is { Length: > 0 } token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
