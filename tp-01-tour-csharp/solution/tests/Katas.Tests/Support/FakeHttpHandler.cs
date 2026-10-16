using System.Net;

namespace Katas.Tests.Support;

/// <summary>
/// A fake HTTP backend: serves fixed content after a delay and records how many requests overlap.
/// Lets the async tests run without network access, and fast.
/// </summary>
public sealed class FakeHttpHandler : HttpMessageHandler
{
    private readonly Dictionary<Uri, (string Content, TimeSpan Delay)> _pages = [];
    private int _inFlight;

    public int MaxConcurrentRequests { get; private set; }

    public FakeHttpHandler Serve(string url, string content, TimeSpan delay)
    {
        _pages[new Uri(url)] = (content, delay);
        return this;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var inFlight = Interlocked.Increment(ref _inFlight);
        lock (_pages)
        {
            MaxConcurrentRequests = Math.Max(MaxConcurrentRequests, inFlight);
        }

        try
        {
            if (!_pages.TryGetValue(request.RequestUri!, out var page))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            await Task.Delay(page.Delay, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(page.Content) };
        }
        finally
        {
            Interlocked.Decrement(ref _inFlight);
        }
    }
}
