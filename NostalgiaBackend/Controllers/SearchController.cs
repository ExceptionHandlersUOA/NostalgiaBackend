using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Models;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController(PostContext context) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Post>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var posts = await context.Posts.Include(p => p.Media).Where(p => p.Categories.Contains(query) || p.SourceUrl.Contains(query) ||
                p.Title.Contains(query) || p.Body.Contains(query) || p.Description.Contains(query)).ToListAsync();

            return Ok(posts);
        }
    }
}
