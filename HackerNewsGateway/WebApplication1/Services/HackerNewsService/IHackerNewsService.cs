using HackerNewsGateway.Dtos;

namespace HackerNewsGateway.Services
{
    public interface IHackerNewsService
    {
        IAsyncEnumerable<HackerNewsStoryDto> GetBestStoriesAsync(int n);
    }
}