using Shared.Enums;

namespace Shared.Models.Database;

public class Feed
{
    public int FeedId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = [];
    public Platform Platform { get; set; } = Platform.Unknown;
}