namespace Katas.Exercise06;

/// <summary>
/// Exercise 06: async/await, concurrency and cancellation.
/// </summary>
public sealed class PageFetcher(HttpClient http)
{
    // Provided: the HttpClient to download with. In the tests, it talks to a fake server.
    private readonly HttpClient _http = http;

    /// <summary>
    /// Downloads every page concurrently and returns their sizes in characters, in the order of <paramref name="uris"/>.
    /// </summary>
    public Task<IReadOnlyList<int>> FetchSizesAsync(IReadOnlyList<Uri> uris, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO Exercise 06");

    /// <summary>
    /// Same as <see cref="FetchSizesAsync"/>, but gives up on a page after <paramref name="timeout"/>
    /// and returns -1 for it. Cancelling <paramref name="cancellationToken"/> still cancels everything.
    /// </summary>
    public Task<IReadOnlyList<int>> FetchSizesWithTimeoutAsync(
        IReadOnlyList<Uri> uris, TimeSpan timeout, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO Exercise 06");
}
