using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using Shared.Enums;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace NostalgiaBackend.Controllers
{
    public class AddCustomPostRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public List<CustomMedia> Media { get; set; } = [];
        public string Body { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public List<string> Categories { get; set; } = [];
        public bool Favourited { get; set; } = false;
    }

    [ApiController]
    [Route("api/feed/{feedId}/post/custom")]
    public class CustomPostController(PostContext context) : Controller
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> PostAsync([FromRoute] int feedId, [FromBody] AddCustomPostRequest customPost)
        {
            var feed = await context.Feeds.FindAsync(feedId);

            if (feed == null || feed.Platform != Platform.Custom)
            {
                return BadRequest();
            }

            var mediaList = customPost.Media.Select(m => new Media
            {
                Type = m.Type,
                FileName = m.FileName
            }).ToList();

            var post = new Post()
            {
                Title = customPost.Title,
                Description = customPost.Description,
                Body = customPost.Body,
                SourceUrl = customPost.SourceUrl,
                LastUpdated = customPost.LastUpdated,
                PublishedAt = customPost.PublishedAt,
                Media = mediaList,
                Categories = customPost.Categories ?? [],
                Favourited = customPost.Favourited,
            };

            context.Posts.Add(post);
            context.Media.AddRange(mediaList);

            feed.Posts.Add(post);

            await context.SaveChangesAsync();

            return Ok(post.PostId);
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
