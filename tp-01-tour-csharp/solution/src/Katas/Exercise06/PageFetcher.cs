namespace Katas.Exercise06;

/// <summary>
/// Exercise 06: async/await, concurrency and cancellation.
/// </summary>
/// <remarks>
/// A <see cref="Task{TResult}"/> is close to a JavaScript Promise. Unlike Rust, there is no runtime to pick:
/// continuations run on the built-in thread pool.
/// </remarks>
public sealed class PageFetcher(HttpClient http)
{
    /// <summary>
    /// Downloads every page concurrently and returns their sizes in characters, in the order of <paramref name="uris"/>.
    /// </summary>
    public async Task<IReadOnlyList<int>> FetchSizesAsync(IReadOnlyList<Uri> uris, CancellationToken cancellationToken = default)
    {
        // Start every download first, then await them together: the requests overlap.
        // Awaiting inside a foreach would download the pages one after the other.
        var downloads = uris.Select(uri => http.GetStringAsync(uri, cancellationToken));
        var pages = await Task.WhenAll(downloads);
        return [.. pages.Select(page => page.Length)];
    }

    /// <summary>
    /// Same as <see cref="FetchSizesAsync"/>, but gives up on a page after <paramref name="timeout"/>
    /// and returns -1 for it. Cancelling <paramref name="cancellationToken"/> still cancels everything.
    /// </summary>
    public async Task<IReadOnlyList<int>> FetchSizesWithTimeoutAsync(
        IReadOnlyList<Uri> uris, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var sizes = await Task.WhenAll(uris.Select(uri => FetchSizeOrTimeoutAsync(uri, timeout, cancellationToken)));
        return sizes;
    }

    private async Task<int> FetchSizeOrTimeoutAsync(Uri uri, TimeSpan timeout, CancellationToken cancellationToken)
    {
        // One timeout per page, linked to the caller's token so that both can stop the request.
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(timeout);

        try
        {
            var page = await http.GetStringAsync(uri, timeoutSource.Token);
            return page.Length;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Only our timeout fired: the caller did not ask to stop, so we report the page as missing.
            return -1;
        }
    }
}
