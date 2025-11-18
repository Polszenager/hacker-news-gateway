using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsGateway.Services.Cache
{
    public class MemoryCacheService : ICache, IDisposable
    {
        private const string BestIdsKey = "hn:beststoryids";
        private const string BestItemsKey = "hn:beststoryitems";

        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _options;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
        }

        public async Task<bool> IdInCacheAsync(int storyId, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (!_memoryCache.TryGetValue(BestIdsKey, out List<int>? cachedIds) || cachedIds == null)
                    return false;

                return cachedIds.FindIndex(x => x == storyId) != -1;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SetStoriesAsync(IReadOnlyList<HackerNewsItem> items, CancellationToken cancellationToken = default)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                _memoryCache.Set(BestIdsKey, items.Select(i => i.Id).ToList(), _options);
                _memoryCache.Set(BestItemsKey, items.Select(Clone).ToList(), _options);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<HackerNewsItem?> GetStoryFromCacheAsync(int storyId, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_memoryCache.TryGetValue(BestItemsKey, out List<HackerNewsItem>? items) && items != null)
                {
                    return Clone(items.Find(x => x.Id == storyId));
                }

                return null;
            }
            finally
            {
                _lock.Release();
            }
        }

        private static HackerNewsItem Clone(HackerNewsItem? src)
        {
            if (src == null) return null!;
            return new HackerNewsItem
            {
                Id = src.Id,
                Title = src.Title,
                Url = src.Url,
                By = src.By,
                Time = src.Time,
                Score = src.Score,
                Descendants = src.Descendants,
                Type = src.Type
            };
        }

        public void Dispose()
        {
            _lock?.Dispose();
        }
    }
}