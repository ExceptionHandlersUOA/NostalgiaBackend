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
        public async Task<ActionResult<Dictionary<string, List<Post>>>> GetAsync()
        {
            return await context.Posts
                .SelectMany(p => p.Categories, (p, c) => new { Category = c, Post = p })
                .GroupBy(x => x.Category)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Post).ToList());
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
