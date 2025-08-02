using HoverthArchiver;
using Microsoft.AspNetCore.Mvc;
using Shared.Database;

namespace NostalgiaBackend.Controllers
{
    [Controller]
    [Route("api/addFeed")]
    public class AddFeed(PostContext context)
    {
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string url)
        {
            var feed = await HoverthInput.RssAsync(url);
            context.Add(feed);
            return new OkResult();
        }
    }
}
