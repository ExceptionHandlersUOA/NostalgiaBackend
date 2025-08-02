using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Models;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController(PostContext context) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetAsync([FromRoute] string categoryName)
        {
            return await context.Posts
                .Where(p => p.Categories.Contains(categoryName))
                .Distinct()
                .ToListAsync();
        }

        [HttpGet("{categoryName}")]
        public async Task<ActionResult<List<Post>>> GetAsyncPosts([FromRoute] string categoryName)
        {
            return await context.Posts
                .Where(p => p.Categories.Contains(categoryName))
                .Include(p => p.Media)
                .ToListAsync();
        }
    }
}
