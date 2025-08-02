using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using Shared.Models;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/feed/{feedId}/post/custom")]
    public class CustomPostController(PostContext context) : Controller
    {
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromRoute] int feedId, [FromBody] AddCustomPostRequest customFeed)
        {
            var feed = await context.Feeds.FindAsync(feedId);

            if (feed == null)
            {
                return BadRequest("Post not found");
            }

            var post = new Post()
            {
                Title = customFeed.Title,
                Description = customFeed.Description,
                Body = customFeed.Body,
                SourceUrl = customFeed.SourceUrl,
                LastUpdated = customFeed.LastUpdated,
                PublishedAt = customFeed.PublishedAt,
                Categories = customFeed.Categories ?? [],
                Favourited = customFeed.Favourited,
            };

            context.Posts.Add(post);

            feed.Posts.Add(post);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
