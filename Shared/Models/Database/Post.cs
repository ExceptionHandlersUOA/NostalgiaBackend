namespace Shared.Models.Database
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public List<Media> Media { get; set; } = [];
        public string Category { get; set; } = string.Empty;
        public bool Favourited { get; set; } = false;
    }
}
