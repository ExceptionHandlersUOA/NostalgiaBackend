using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
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
        public string Body { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public List<string> Categories { get; set; } = [];
        public bool Favourited { get; set; } = false;
    }

    [ApiController]
    [Route("api/feed/{feedId}/post")]
    public class PostController(PostContext context) : Controller
    {
        [HttpGet("{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Post>> GetAsync([FromRoute] int postId)
        {
            var post = await context.Posts
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
                return BadRequest();

            return Ok(post);
        }
    }
}
