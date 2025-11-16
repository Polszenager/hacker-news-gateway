using HackerNewsGateway.Dtos;

namespace HackerNewsGateway.Services
{
    public interface IHackerNewsService
    {
        Task<IEnumerable<HackerNewsStoryDto>> GetBestStoriesAsync(int n);
    }
}

