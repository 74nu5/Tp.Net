namespace Tickets.Api.Diagnostics;

/// <summary>
/// Exercise 01: times every request, logs it and returns the duration in the X-Request-Duration-Ms header.
/// </summary>
public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    public const string HeaderName = "X-Request-Duration-Ms";

    // Injected by the container: use it to write the log line.
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO Exercise 01: start a stopwatch, add the header just before the response starts,
        // then log the method, the path, the status code and the duration.
        await next(context);
    }
}
