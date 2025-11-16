using FluentAssertions;
using HackerNewsGateway.Dtos;
using HackerNewsGateway.Services;
using HackerNewsGateway.Tests.TestUtilities;

namespace HackerNewsGateway.Tests
{
    public class HackerNewsServiceTests
    {
        private readonly MockHttpMessageHandler _mockHttp;
        private readonly HttpClient _httpClient;
        private readonly HackerNewsService _service;

        public HackerNewsServiceTests()
        {
            _mockHttp = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHttp);
            _service = new HackerNewsService(_httpClient);
        }

        [Fact]
        public async Task GetBestStoriesAsync_WhenNIsZero_ReturnsEmptyList()
        {
            var storyIds = new List<int> { 1, 2, 3 };

            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/beststories.json")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(storyIds));

            var result = new List<HackerNewsStoryDto>();
            await foreach (var s in _service.GetBestStoriesAsync(0))
                result.Add(s);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetBestStoriesAsync_WhenResponseIsNull_ReturnsEmptyList()
        {
            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/beststories.json")
                .Respond("application/json", "null");

            var result = new List<HackerNewsStoryDto>();
            await foreach (var s in _service.GetBestStoriesAsync(3))
                result.Add(s);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetBestStoriesAsync_WhenMoreStoriesThanRequested_ReturnsTopNInOrder()
        {
            var storyIds = new List<int> { 2, 4, 3, 5, 1 };
            var story1 = new { id = 1, title = "Story 1", url = "https://example.com/1", by = "user1", time = 1609459200, score = 50, descendants = 10, type = "story" };
            var story2 = new { id = 2, title = "Story 2", url = "https://example.com/2", by = "user2", time = 1609459260, score = 300, descendants = 20, type = "story" };
            var story3 = new { id = 3, title = "Story 3", url = "https://example.com/3", by = "user3", time = 1609459320, score = 150, descendants = 30, type = "story" };
            var story4 = new { id = 4, title = "Story 4", url = "https://example.com/4", by = "user4", time = 1609459380, score = 250, descendants = 40, type = "story" };
            var story5 = new { id = 5, title = "Story 5", url = "https://example.com/5", by = "user5", time = 1609459440, score = 100, descendants = 50, type = "story" };

            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/beststories.json")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(storyIds));
            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/item/1.json")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(story1));
            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/item/2.json")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(story2));
            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/item/3.json")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(story3));
            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/item/4.json")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(story4));
            _mockHttp
                .When("https://hacker-news.firebaseio.com/v0/item/5.json")
                .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(story5));

            var result = new List<HackerNewsStoryDto>();
            await foreach (var s in _service.GetBestStoriesAsync(3))
                result.Add(s);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeInDescendingOrder(s => s.Score);
            result.First().Score.Should().Be(300);
            result.Skip(1).First().Score.Should().Be(250);
            result.Last().Score.Should().Be(150);
        }
    }
}