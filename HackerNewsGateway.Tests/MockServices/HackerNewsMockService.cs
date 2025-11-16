using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNewsGateway.Dtos;
using HackerNewsGateway.Services;

namespace HackerNewsGateway.Tests
{
    public class HackerNewsMockService : IHackerNewsService
    {
        public Task<IEnumerable<HackerNewsStoryDto>> GetBestStoriesAsync(int n)
        {
            return Task.FromResult<IEnumerable<HackerNewsStoryDto>>(new List<HackerNewsStoryDto>
            {
                new HackerNewsStoryDto
                {
                    Title = "A uBlock Origin update was rejected from the Chrome Web Store",
                    Uri = "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
                    PostedBy = "ismaildonmez",
                    Time = "2019-10-12T13:43:01+00:00",
                    Score = 1716,
                    CommentCount = 572
                }
            });
        }
    }
}
