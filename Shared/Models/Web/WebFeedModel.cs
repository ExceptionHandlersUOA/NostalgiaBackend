namespace Shared.Models;

public class WebFeedModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Url { get; set; }
    public List<WebPostModel> posts { get; set; } = [];
}