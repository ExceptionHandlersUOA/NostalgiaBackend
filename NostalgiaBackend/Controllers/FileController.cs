using Microsoft.AspNetCore.Mvc;
using Shared.Files;

namespace NostalgiaBackend.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            var extension = Path.GetExtension(file.FileName);
            using var stream = file.OpenReadStream();
            
            var fileName = await StaticFiles.AddFileToSystem(stream, extension);
            
            return CreatedAtAction(nameof(GetAsync), new { fileName }, new { fileName });
        }

        [HttpGet("{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string fileName)
        {
            try
            {
                var fileBytes = await StaticFiles.GetFileOnSystem(fileName);
                var contentType = GetContentType(fileName);
                return File(fileBytes, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{fileName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] string fileName)
        {
            try
            {
                StaticFiles.DeleteFileOnSystem(fileName);
                return NoContent();
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
