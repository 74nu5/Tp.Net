namespace Tickets.Api.Diagnostics;

/// <summary>
/// Exercise 01: times every request, logs it and returns the duration in the X-Request-Duration-Ms header.
/// </summary>
public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    public const string HeaderName = "X-Request-Duration-Ms";

    // Injected by the container: use _logger to write the log line. (Using the logger parameter as well
    // would capture it twice, warning CS9124; the solution keeps the parameter and drops the field.)
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO Exercise 01: start a stopwatch, add the header just before the response starts,
        // then log the method, the path, the status code and the duration.
        await next(context);
    }
}
