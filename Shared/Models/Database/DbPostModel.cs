using Shared.Enums;

namespace Shared.Models.Database
{
    public class DbPostModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SourceUrl { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime PublishedAt { get; set; }
        public List<DbWebMediaModel> Media { get; set; } = [];
        public Platform Platform { get; set; }
    }
}
