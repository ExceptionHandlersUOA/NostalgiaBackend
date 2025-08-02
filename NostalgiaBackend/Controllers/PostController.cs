using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Models;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/post/{postId}")]
    public class PostController(PostContext context) : Controller
    {
        [HttpGet]
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

        [HttpPost("favourite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Post>> FavouriteAsync([FromRoute] int postId)
        {
            var post = await context.Posts
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
                return BadRequest();

            post.Favourited = true;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("unfavourite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Post>> UnfavouriteAsync([FromRoute] int postId)
        {
            var post = await context.Posts
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
                return BadRequest();

            post.Favourited = false;

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
