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

        public async Task<IEnumerable<HackerNewsStoryDto>> GetBestStoriesAsync(int n)
        {
            var bestStoriesResponse = await _httpClient.GetStringAsync($"{BaseUrl}beststories.json");
            var storyIds = JsonConvert.DeserializeObject<List<int>>(bestStoriesResponse) ?? new List<int>();

            var stories = new List<HackerNewsStoryDto>();
            foreach (var storyId in storyIds.Take(n))
            {
                var itemResponse = await _httpClient.GetStringAsync($"{BaseUrl}item/{storyId}.json");
                var item = JsonConvert.DeserializeObject<HackerNewsItem>(itemResponse);
                
                if (item != null && item.Type == "story")
                {
                    stories.Add(MapToDto(item));
                }
            }

            return stories.OrderByDescending(s => s.Score).Take(n);
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

