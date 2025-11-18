using HackerNewsGateway.Dtos;
using HackerNewsGateway.Services.Cache;
using Newtonsoft.Json;

namespace HackerNewsGateway.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly ICache? _cache;

        public HackerNewsService(HttpClient httpClient, ICache? cache = null)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async IAsyncEnumerable<HackerNewsStoryDto> GetBestStoriesAsync(int n)
        {
            if (n == 0) yield break;

            // relative request uses HttpClient.BaseAddress configured in Program.cs
            var bestStoriesResponse = await _httpClient.GetStringAsync("beststories.json");
            var storyIds = JsonConvert.DeserializeObject<List<int>>(bestStoriesResponse) ?? new List<int>();
            if (storyIds.Count == 0) yield break;

            var limitedStoryIds = storyIds.Take(n).ToList();

            var finalItems = new List<HackerNewsItem>();
            foreach (var storyId in storyIds.Take(n))
            {
                if (_cache != null && await _cache.IdInCacheAsync(storyId))
                {
                    var cachedItem = await _cache.GetStoryFromCacheAsync(storyId);
                    if (cachedItem != null)
                    {
                        finalItems.Add(cachedItem);
                        yield return MapToDto(cachedItem);
                        continue;
                    }
                }

                var itemResponse = await _httpClient.GetStringAsync($"item/{storyId}.json");
                var item = JsonConvert.DeserializeObject<HackerNewsItem>(itemResponse);
                if (item != null && item.Type == "story")
                {
                    if (item.Id == 0) item.Id = storyId;
                    finalItems.Add(item);
                    yield return MapToDto(item);
                }
            }

            if (_cache != null && finalItems.Count > 0)
            {
                await _cache.SetStoriesAsync(finalItems);
            }
        }

        private static HackerNewsStoryDto MapToDto(HackerNewsItem item)
        {
            return new HackerNewsStoryDto
            {
                Title = item.Title ?? string.Empty,
                Uri = item.Url ?? string.Empty,
                PostedBy = item.By ?? string.Empty,
                Time = DateTimeOffset.FromUnixTimeSeconds(item.Time).ToString("yyyy-MM-ddTHH:mm:ss+00:00"),
                Score = item.Score,
                CommentCount = item.Descendants ?? 0
            };
        }
    }
}