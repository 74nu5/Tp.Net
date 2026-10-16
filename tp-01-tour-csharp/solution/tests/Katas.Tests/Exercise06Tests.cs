using Katas.Exercise06;
using Katas.Tests.Support;

namespace Katas.Tests.Exercise06;

public class PageFetcherTests
{
    private static readonly Uri[] Pages = [new("https://a.test/"), new("https://b.test/"), new("https://c.test/")];

    private static FakeHttpHandler ThreePages(TimeSpan delay) => new FakeHttpHandler()
        .Serve("https://a.test/", "alpha", delay)
        .Serve("https://b.test/", "bravo!", delay)
        .Serve("https://c.test/", "charlie", delay);

    [Fact]
    public async Task FetchSizes_ReturnsTheSizesInOrder()
    {
        var fetcher = new PageFetcher(new HttpClient(ThreePages(TimeSpan.FromMilliseconds(10))));

        var sizes = await fetcher.FetchSizesAsync(Pages, TestContext.Current.CancellationToken);

        Assert.Equal([5, 6, 7], sizes);
    }

    [Fact]
    public async Task FetchSizes_DownloadsThePagesConcurrently()
    {
        var handler = ThreePages(TimeSpan.FromMilliseconds(200));
        var fetcher = new PageFetcher(new HttpClient(handler));

        await fetcher.FetchSizesAsync(Pages, TestContext.Current.CancellationToken);

        Assert.Equal(3, handler.MaxConcurrentRequests);
    }

    [Fact]
    public async Task FetchSizes_StopsWhenCancelled()
    {
        var fetcher = new PageFetcher(new HttpClient(ThreePages(TimeSpan.FromSeconds(30))));
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => fetcher.FetchSizesAsync(Pages, cancellation.Token));
    }

    [Fact]
    public async Task FetchSizesWithTimeout_ReportsSlowPagesAsMissing()
    {
        var handler = new FakeHttpHandler()
            .Serve("https://a.test/", "alpha", TimeSpan.FromMilliseconds(10))
            .Serve("https://b.test/", "bravo!", TimeSpan.FromSeconds(30))
            .Serve("https://c.test/", "charlie", TimeSpan.FromMilliseconds(10));
        var fetcher = new PageFetcher(new HttpClient(handler));

        var sizes = await fetcher.FetchSizesWithTimeoutAsync(Pages, TimeSpan.FromMilliseconds(300), TestContext.Current.CancellationToken);

        Assert.Equal([5, -1, 7], sizes);
    }

    [Fact]
    public async Task FetchSizesWithTimeout_StillStopsWhenTheCallerCancels()
    {
        var fetcher = new PageFetcher(new HttpClient(ThreePages(TimeSpan.FromSeconds(30))));
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => fetcher.FetchSizesWithTimeoutAsync(Pages, TimeSpan.FromSeconds(10), cancellation.Token));
    }
}
