using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models.Web;

namespace NostalgiaBackend.Controllers
{
    [Controller]
    [Route("api/getPosts")]
    public class GetPosts
    {
        [HttpGet]
        public List<WebPost> Get()
        {
            return SampleData.GetSamplePosts();
        }
    }
}
