using Microsoft.AspNetCore.Mvc;
using Shared.Files;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    public class GetFile() : ControllerBase
    {
        [HttpGet("api/getFile/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFileAsync(string fileName)
        {
            var fileBytes = await StaticFiles.GetFileOnSystem(fileName);
            return File(fileBytes, Path.GetFileName(fileName));
        }
    }
}
