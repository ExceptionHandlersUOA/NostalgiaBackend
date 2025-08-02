using Shared.Models.Database;

namespace Shared.Models.Web
{
    public class WebPost(Post post)
    {
        public int PostId { get; set; } = post.PostId;
        public string Title { get; set; } = post.Title;
        public string Description { get; set; } = post.Description;
        public string SourceUrl { get; set; } = post.SourceUrl;
        public DateTime LastUpdated { get; set; } = post.LastUpdated;
        public DateTime PublishedAt { get; set; } = post.PublishedAt;
        public List<WebMedia> Media { get; set; } = [.. post.Media.Select(media => new WebMedia(media))];
    }
}
