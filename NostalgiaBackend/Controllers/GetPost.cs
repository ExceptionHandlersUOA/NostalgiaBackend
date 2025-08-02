using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Models.Web;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/getPost/{id}")]
    public class GetPost(PostContext _context) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WebPost>> GetAsync([FromRoute] int id)
        {
            var post = await _context.Posts
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
                return BadRequest();

            return Ok(new WebPost(post));
        }
    }
}
