using System.Diagnostics;

namespace Tickets.Api.Diagnostics;

/// <summary>
/// Exercise 01: times every request, logs it and returns the duration in the X-Request-Duration-Ms header.
/// </summary>
/// <remarks>
/// A convention-based middleware: a constructor taking the next delegate, and an InvokeAsync method.
/// Dependencies with a shorter lifetime than the app (scoped services) go in InvokeAsync, not in the constructor.
/// </remarks>
public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    public const string HeaderName = "X-Request-Duration-Ms";

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Headers can only change before the body starts: OnStarting runs just before that moment.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = stopwatch.ElapsedMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return Task.CompletedTask;
        });

        await next(context);

        // Structured logging: Method, Path... become searchable fields, not just text.
        logger.LogInformation(
            "{Method} {Path} answered {StatusCode} in {ElapsedMs} ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
