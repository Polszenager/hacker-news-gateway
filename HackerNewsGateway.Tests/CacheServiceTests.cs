using HackerNewsGateway.Services;
using HackerNewsGateway.Services.Cache;
using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;

namespace HackerNewsGateway.Tests
{
    public class CacheServiceTests
    {
        private IMemoryCache CreateMemoryCache() => new MemoryCache(new MemoryCacheOptions());

        [Fact]
        public async Task IsCacheUpToDate_WhenEmpty_ReturnsFalse()
        {
            var mem = CreateMemoryCache();
            var svc = new MemoryCacheService(mem);
            var result = false;
            var ids = new List<int> { 1, 2, 3 };
            foreach (var item in ids)
            {
                result = await svc.IdInCacheAsync(item);
            }

            result.Should().BeFalse();
        }

        [Fact]
        public async Task SetStories_Then_IsCacheUpToDate_And_GetStories_ReturnsValues()
        {
            var mem = CreateMemoryCache();
            var svc = new MemoryCacheService(mem);

            var items = new List<HackerNewsItem>
            {
                new HackerNewsItem { Id = 1, Title = "A" },
                new HackerNewsItem { Id = 2, Title = "B" },
                new HackerNewsItem { Id = 3, Title = "C" }
            };

            await svc.SetStoriesAsync(items);

            var ids = new List<int> { 1, 2, 3 };
            var upToDate = await svc.IdInCacheAsync(1);
            upToDate.Should().BeTrue();

            var cached = await svc.GetStoryFromCacheAsync(1);
            cached.Id.Should().Be(1);
        }
    }
}