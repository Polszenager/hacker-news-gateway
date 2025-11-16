using HackerNewsGateway.Dtos;
using HackerNewsGateway.Services;


namespace HackerNewsGateway.Tests
{
    public class HackerNewsMockService : IHackerNewsService
    {
        IAsyncEnumerable<HackerNewsStoryDto> IHackerNewsService.GetBestStoriesAsync(int n)
        {
            var stories = new List<HackerNewsStoryDto>
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
            };

            // Convert the list to an async enumerable
            return stories.ToAsyncEnumerable();
        }
    }
}
