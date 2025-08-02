using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Models.Web;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/getFeeds")]
    public class GetFeeds(PostContext _context) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<WebFeed>>> GetAsync()
        {
            var feeds = await _context.Feeds
                .Include(f => f.Posts)
                    .ThenInclude(p => p.Media)
                .ToListAsync();

            return Ok(feeds.Select(f => new WebFeed(f)).ToList());
        }
    }
}
