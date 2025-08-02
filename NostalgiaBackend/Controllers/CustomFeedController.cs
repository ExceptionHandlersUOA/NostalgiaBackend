using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Enums;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace NostalgiaBackend.Controllers
{
    public class AddCustomFeedRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Url]
        public string ImageUrl { get; set; } = string.Empty;
        [Url]
        public string Url { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/feed/custom")]
    public class CustomFeedController(PostContext context) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> PostAsync([FromBody] AddCustomFeedRequest customFeed)
        {
            var feed = new Feed
            {
                Title = customFeed.Title,
                Description = customFeed.Description,
                ImageUrl = customFeed.ImageUrl,
                Url = customFeed.Url,
                Platform = Platform.Custom
            };

            context.Add(feed);

            await context.SaveChangesAsync();

            return Ok(feed.FeedId);
        }

        [HttpDelete("{feedId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(int feedId)
        {
            var feed = await context.Feeds
                .Include(f => f.Posts)
                .ThenInclude(p => p.Media)
                .FirstOrDefaultAsync(f => f.FeedId == feedId);

            if (feed == null || feed.Platform != Platform.Custom)
            {
                return BadRequest();
            }

            context.Remove(feed);

            foreach (var post in feed.Posts)
            {
                context.Posts.Remove(post);

                foreach (var media in post.Media)
                {
                    context.Media.Remove(media);
                }
            }

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
