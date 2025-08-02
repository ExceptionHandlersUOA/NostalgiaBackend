using Shared.Enums;

namespace Shared.Models.Web
{
    public class WebMedia
    {
        public int MediaId { get; set; }
        public FileType Type { get; set; }
        public string FileUrl { get; set; }
    }
}
