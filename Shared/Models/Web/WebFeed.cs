using Shared.Enums;
using Shared.Models.Database;

namespace Shared.Models.Web;

public class WebFeed(Feed feed)
{
    public int FeedId { get; set; } = feed.FeedId;
    public string Title { get; set; } = feed.Title;
    public string Description { get; set; } = feed.Description;
    public string ImageUrl { get; set; } = feed.ImageUrl;
    public string Url { get; set; } = feed.Url;
    public List<WebPost> Posts { get; set; } = [.. feed.Posts.Select(post => new WebPost(post))];
    public Platform Platform { get; set; } = feed.Platform;
}