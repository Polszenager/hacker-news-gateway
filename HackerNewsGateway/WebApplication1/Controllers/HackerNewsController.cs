using HackerNewsGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;

        public HackerNewsController(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("best/{n}")]
        public async Task<IActionResult> GetBestStories([FromRoute] int n)
        {
            var result = await _hackerNewsService.GetBestStoriesAsync(n);
            return Ok(result);
        }
    }
}
