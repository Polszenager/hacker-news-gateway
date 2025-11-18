namespace HackerNewsGateway.Services.Cache
{
    public interface ICache
    {
        Task<bool> IdInCacheAsync(int storyId, CancellationToken cancellationToken = default);
        Task SetStoriesAsync(IReadOnlyList<HackerNewsItem> items, CancellationToken cancellationToken = default);
        Task<HackerNewsItem?> GetStoryFromCacheAsync(int storyId, CancellationToken cancellationToken = default);
    }
}