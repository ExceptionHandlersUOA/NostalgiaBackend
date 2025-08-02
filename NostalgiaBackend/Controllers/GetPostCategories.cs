using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using System.ComponentModel.DataAnnotations;

namespace NostalgiaBackend.Controllers
{
    public class CategoryRequest
    {
        [Required]
        public string Category { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/getPost/{postId}/categories")]
    public class GetPostCategories(PostContext _context) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddAsync([FromRoute] int postId, [FromBody, Required] CategoryRequest request)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return BadRequest("Post not found");
            }

            if (post.Categories.Contains(request.Category))
            {
                return BadRequest("Category already exists");
            }

            post.Categories.Add(request.Category);

            _context.Posts.Update(post);

            return Ok();
        }


        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync([FromRoute] int postId, [FromBody, Required] CategoryRequest request)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return BadRequest("Post not found");
            }

            if (!post.Categories.Contains(request.Category))
            {
                return BadRequest("Category already exists");
            }

            post.Categories.Remove(request.Category);

            _context.Posts.Update(post);

            return Ok();
        }
    }
}
