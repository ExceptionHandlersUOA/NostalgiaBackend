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
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody, Required] AddFeedRequest request)
        {
            if (string.IsNullOrEmpty(request.Url))
            {
                return BadRequest("URL is required");
            }

            var feed = await HoverthInput.RssAsync(request.Url);
            context.Add(feed);
            await context.SaveChangesAsync();
            
            return Ok();
        }
    }
}