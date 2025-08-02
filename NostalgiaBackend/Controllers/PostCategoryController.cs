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
    public class PostCategoryController(PostContext context) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAsync([FromRoute] int postId, [FromBody, Required] CategoryRequest request)
        {
            var post = await context.Posts.FindAsync(postId);

            if (post == null)
            {
                return BadRequest("Post not found");
            }

            if (post.Categories.Contains(request.Category))
            {
                return BadRequest("Category already exists");
            }

            post.Categories.Add(request.Category);

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync([FromRoute] int postId, [FromBody, Required] CategoryRequest request)
        {
            var post = await context.Posts.FindAsync(postId);

            if (post == null)
            {
                return BadRequest("Post not found");
            }

            if (!post.Categories.Contains(request.Category))
            {
                return BadRequest("Category already exists");
            }

            post.Categories.Remove(request.Category);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
