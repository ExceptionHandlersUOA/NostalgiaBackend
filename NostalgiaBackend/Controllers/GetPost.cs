using Microsoft.AspNetCore.Mvc;
using Shared.Database;
using Shared.Models.Web;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/getPost/{id}")]
    public class GetPost(PostContext _context)
    {
        [HttpGet]
        public WebPost Get([FromRoute] string id)
        {
            return new WebPost(_context.Posts.Find(id));
        }
    }
}
