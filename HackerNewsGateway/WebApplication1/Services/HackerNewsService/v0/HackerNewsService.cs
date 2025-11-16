using HackerNewsGateway.Dtos;
using Newtonsoft.Json;

namespace HackerNewsGateway.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0/";
        private readonly HttpClient _httpClient;

        public HackerNewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async IAsyncEnumerable<HackerNewsStoryDto> GetBestStoriesAsync(int n)
        {
            if (n == 0)
            {
                yield break;
            }

            var bestStoriesResponse = await _httpClient.GetStringAsync($"{BaseUrl}beststories.json");
            var storyIds = JsonConvert.DeserializeObject<List<int>>(bestStoriesResponse) ?? new List<int>();

            if (storyIds.Count == 0)
            {
                yield break;
            }

            foreach (var storyId in storyIds.Take(n))
            {
                var itemResponse = await _httpClient.GetStringAsync($"{BaseUrl}item/{storyId}.json");
                var item = JsonConvert.DeserializeObject<HackerNewsItem>(itemResponse);

                if (item != null && item.Type == "story")
                {
                    yield return MapToDto(item);
                }
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