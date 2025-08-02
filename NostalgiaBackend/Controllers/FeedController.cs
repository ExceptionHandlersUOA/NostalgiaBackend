using HoverthArchiver;
using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace NostalgiaBackend.Controllers
{
    public class AddFeedUrlRequest
    {
        [Required]
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

            context.Feeds.Remove(feed);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}