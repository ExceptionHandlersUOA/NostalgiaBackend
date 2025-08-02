namespace Shared.Models.Database;

public class Feed
{
    public int FeedId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Url { get; set; }
    public List<Post> Posts { get; set; } = [];
}