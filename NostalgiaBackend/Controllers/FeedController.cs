using HoverthArchiver;
using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using Shared.Enums;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace NostalgiaBackend.Controllers
{
    public class AddFeedUrlRequest
    {
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
    }

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
    [Route("api/feed")]
    public class FeedController(PostContext context) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody, Required] AddFeedUrlRequest request)
        {
            if (string.IsNullOrEmpty(request.Url))
            {
                return BadRequest("URL is required");
            }

            var feed = await HoverthInput.AddFeed(request.Url);

            context.Feeds.Add(feed);

            await context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpPost("custom")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAsync([FromBody] AddCustomFeedRequest customFeed)
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

            return Ok();
        }

        [HttpDelete("{feedId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int feedId)
        {
            var feed = await context.Feeds.FindAsync(feedId);

            if (feed == null)
            {
                return BadRequest("Feed not found");
            }

            if (feed.Platform != Platform.Custom)
            {
                return BadRequest("Feed is not a custom feed!");
            }

            context.Feeds.Remove(feed);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}