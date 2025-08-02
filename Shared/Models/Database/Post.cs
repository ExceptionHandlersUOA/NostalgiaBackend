using Shared.Enums;

namespace Shared.Models.Database
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SourceUrl { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime PublishedAt { get; set; }
        public List<Media> Media { get; set; } = [];
        public Platform Platform { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool Favourited { get; set; } = false;
    }
}
