using Shared.Enums;

namespace Shared.Models.Web
{
    public class WebPost
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SourceUrl { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime PublishedAt { get; set; }
        public List<WebMedia> Media { get; set; } = [];
        public Platform Platform { get; set; }
    }
}
