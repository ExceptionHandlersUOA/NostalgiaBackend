using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;
using Shared.Database;

namespace NostalgiaBackend.Controllers
{
    public class CustomFeedRequest
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
    [Route("api/addCustomFeed")]
    public class AddCustomFeed(PostContext context) : Controller
    {
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] CustomFeedRequest customFeed)
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
    }
}
