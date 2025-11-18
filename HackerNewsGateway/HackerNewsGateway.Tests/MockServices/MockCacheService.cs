using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsGateway.Services;
using HackerNewsGateway.Services.Cache;

namespace HackerNewsGateway.Tests.MockServices
{
    // Simple in-memory mock of ICache for unit tests.
    public class MockCacheService : ICache
    {
        private List<HackerNewsItem> _items = new List<HackerNewsItem>();

        public Task<IReadOnlyList<HackerNewsItem>> GetStoriesFromCacheAsync(CancellationToken cancellationToken = default)
        {
            // return copies to mimic production behaviour
            var copy = _items.Select(i => new HackerNewsItem
            {
                Id = i.Id,
                Title = i.Title,
                Url = i.Url,
                By = i.By,
                Time = i.Time,
                Score = i.Score,
                Descendants = i.Descendants,
                Type = i.Type
            }).ToList();

            return Task.FromResult((IReadOnlyList<HackerNewsItem>)copy);
        }

        public Task<bool> IsCacheUpToDateAsync(IReadOnlyList<int> storyIds, CancellationToken cancellationToken = default)
        {
            if (storyIds == null) return Task.FromResult(false);
            if (_items.Count != storyIds.Count) return Task.FromResult(false);

            for (int i = 0; i < storyIds.Count; i++)
            {
                if (_items[i].Id != storyIds[i])
                    return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task SetStoriesAsync(IReadOnlyList<HackerNewsItem> items, CancellationToken cancellationToken = default)
        {
            _items = items?.Select(i => new HackerNewsItem
            {
                Id = i.Id,
                Title = i.Title,
                Url = i.Url,
                By = i.By,
                Time = i.Time,
                Score = i.Score,
                Descendants = i.Descendants,
                Type = i.Type
            }).ToList() ?? new List<HackerNewsItem>();

            return Task.CompletedTask;
        }
    }
}
