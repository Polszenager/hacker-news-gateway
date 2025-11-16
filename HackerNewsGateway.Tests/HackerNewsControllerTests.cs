using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HackerNewsGateway.Dtos;
using HackerNewsGateway.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HackerNewsGateway.Tests
{
    public class HackerNewsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HackerNewsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddTransient<IHackerNewsService, HackerNewsMockService>();
                });
            });
        }

        [Fact]
        public async Task GetBestStories_ReturnsMockData()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/HackerNews/best/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<HackerNewsStoryDto[]>();
            data.Should().NotBeNull();
            data.Should().HaveCount(1);
            data![0].Title.Should().Be("A uBlock Origin update was rejected from the Chrome Web Store");
        }
    }
}
