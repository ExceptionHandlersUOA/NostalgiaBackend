using HoverthArchiver;
using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using System.ComponentModel.DataAnnotations;

namespace NostalgiaBackend.Controllers
{
    public class AddFeedRequest
    {
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/addFeed")]
    public class AddFeedController(PostContext context) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody, Required] AddFeedRequest request)
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
    }
}