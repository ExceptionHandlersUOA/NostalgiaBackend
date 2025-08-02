using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Models.Web;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/getFeeds")]
    public class GetFeeds(PostContext _context)
    {
        [HttpGet]
        public List<WebFeed> Get()
        {
            var feeds = _context.Feeds
                .Include(f => f.Posts)
                    .ThenInclude(p => p.Media)
                .ToList();
            return [.. feeds.Select(f => new WebFeed(f))];
        }
    }
}
