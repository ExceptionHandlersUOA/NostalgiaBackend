namespace Shared.Models.Database;

public class DbFeedModel
{
    
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Url { get; set; }
    public List<DbPostModel> posts { get; set; } = [];
    
}