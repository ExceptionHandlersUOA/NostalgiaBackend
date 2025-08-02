using Shared.Enums;

namespace Shared.Models
{
    public class PostModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SourceUrl { get; set; }
        public DateTime PublishedAt { get; set; }
        public List<MediaModel> Media { get; set; } = [];
        public Platform Platform { get; set; }
    }
}
