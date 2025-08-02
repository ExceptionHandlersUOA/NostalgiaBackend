using Microsoft.AspNetCore.Mvc;
using Shared.Files;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/file/{fileName}")]
    public class FileController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string fileName)
        {
            var fileBytes = await StaticFiles.GetFileOnSystem(fileName);
            return File(fileBytes, Path.GetFileName(fileName));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteAsync([FromRoute] string fileName)
        {
            StaticFiles.DeleteFileOnSystem(fileName);
            return Ok();
        }
    }
}
