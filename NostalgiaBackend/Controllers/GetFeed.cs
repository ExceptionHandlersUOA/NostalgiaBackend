using AngleSharp.Io;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Models;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/feed/{feedId}")]
    public class GetFeed(PostContext context) : Controller
    {
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromRoute] int feedId)
        {
            var feed = await context.Feeds.FindAsync(feedId);

            if (feed == null)
            {
                return BadRequest("Feed not found");
            }

            if (feed.Platform != Shared.Enums.Platform.Custom)
            {
                return BadRequest("Feed is not a custom feed!");
            }

            context.Feeds.Remove(feed);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
