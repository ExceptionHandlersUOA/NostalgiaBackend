namespace Shared.Models.Web;

public class WebFeed
{
    public int FeedId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Url { get; set; }
    public List<WebPost> Posts { get; set; } = [];
}