using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using Shared.Enums;
using Shared.Models;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/feed/{feedId}/post/custom")]
    public class CustomPostController(PostContext context) : Controller
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAsync([FromRoute] int feedId, [FromBody] AddCustomPostRequest customFeed)
        {
            var feed = await context.Feeds.FindAsync(feedId);

            if (feed == null || feed.Platform != Platform.Custom)
            {
                return BadRequest();
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

        [HttpDelete("{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync([FromRoute] int feedId, [FromRoute] int postId)
        {
            var feed = await context.Feeds.FindAsync(feedId);

            if (feed == null || feed.Platform != Platform.Custom)
            {
                return BadRequest("Custom feed not found.");
            }

            var post = feed.Posts.FirstOrDefault(p => p.PostId == postId);

            if (post == null)
            {
                return BadRequest("Custom post not found");
            }

            feed.Posts.Remove(post);
            context.Posts.Remove(post);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
