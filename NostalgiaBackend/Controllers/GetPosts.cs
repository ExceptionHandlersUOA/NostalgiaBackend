using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Models;

namespace NostalgiaBackend.Controllers
{
    [Controller]
    [Route("api/getPosts")]
    public class GetPosts
    {
        [HttpGet]
        public List<WebPostModel> Get()
        {
            return SampleData.GetSamplePosts();
        }
    }
}
