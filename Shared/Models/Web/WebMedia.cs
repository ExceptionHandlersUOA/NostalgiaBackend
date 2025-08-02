using Shared.Enums;
using Shared.Models.Database;

namespace Shared.Models.Web
{
    public class WebMedia(Media media)
    {
        public int MediaId { get; set; } = media.MediaId;
        public FileType Type { get; set; } = media.Type;
        public string FileUrl { get; set; } = media.FileName;
    }
}
