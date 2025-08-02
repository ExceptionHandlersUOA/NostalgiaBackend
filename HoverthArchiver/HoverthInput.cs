using System.Net;
using Shared;
using CodeHollow.FeedReader;
using Shared.Enums;
using Shared.Models.Database;
using Feed = Shared.Models.Database.Feed;

namespace HoverthArchiver
{
    public class HoverthInput
    {
        private static HttpClient httpClient = new HttpClient()
        {
            Timeout = new TimeSpan(0,5,0) // 5 minute timeout,
        };
        public static async Task<Feed> AddFeed(string url, Platform platform = Platform.RSS)
        {
            if (url.Contains("reddit.com"))
            {
                return await Reddit(url);
            }

            if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            {
                return await YouTube(url);
            }

            return await RssAsync(url, platform);
        }
        
        
        private static async Task<Feed> Reddit(string url)
        {
            Console.WriteLine(url);
            var rssUrl = url + ".rss";
            //var rss_url = FeedReader.GetFeedUrlsFromUrl(url).First().Url;
            Console.WriteLine(rssUrl);
            return await RssAsync(rssUrl, Platform.Reddit);
        }

        private static async Task<Feed> YouTube(string url)
        {
            // change yt channel link into https://www.youtube.com/feeds/videos.xml?channel_id=UCRC6cNamj9tYAO6h_RXd5xA format
            // https://www.youtube.com/channel/UCRC6cNamj9tYAO6h_RXd5xA

            string channelId = url.Split('/').Last().Split('?').First();
            var rssUrl = "https://www.youtube.com/feeds/videos.xml?channel_id=" + channelId;
            return await RssAsync(rssUrl, Platform.YouTube);
        }

        private string DownloadFile(string url)
        {
            string filename = "/tmp/";

            return filename;
        }

        private static async Task<Feed> RssAsync(string url, Platform platform = Platform.RSS)
        {
            HtmlParser parser = new();
            var feed = await FeedReader.ReadAsync(url);

            Console.WriteLine("Feed Title: " + feed.Title);
            Console.WriteLine("Feed Description: " + feed.Description);
            Console.WriteLine("Feed Image: " + feed.ImageUrl);

            List<Post> postList = new List<Post>();
            
            foreach (var item in feed.Items)
            {
                Console.WriteLine(item.Title + " - " + item.Link);

                var textContent = parser.FlattenText(item.Content);
                var imageUrls = parser.GetImages(item.Content);
                var videoUrls = parser.GetVideos(item.Content);

                List<Media> mediaList = new List<Media>();

                foreach (var image in imageUrls)
                {
                    // Download
                    var media = new Media()
                    {
                        Type = FileType.Image,
                        FileName = "",
                    };
                    mediaList.Add(media);
                }
                
                foreach (var video in videoUrls)
                {
                    // Download
                    var media = new Media()
                    {
                        Type = FileType.Video,
                        FileName = "",
                    };
                    mediaList.Add(media);
                }
                
                var post = new Post()
                {
                    Description = item.Description ?? string.Empty,
                    Title = item.Title ?? string.Empty,
                    SourceUrl = item.Link ?? string.Empty,
                    PublishedAt = item.PublishingDate ?? DateTime.MinValue,
                    Category = string.Empty,
                    Favourited = false,
                    LastUpdated = item.PublishingDate ?? DateTime.MinValue,
                    Media = mediaList,
                    Body = textContent,
                };
                postList.Add(post);
            }

            var feedModel = new Feed()
            {
                Description = feed.Description ?? string.Empty,
                FeedId = 0,
                ImageUrl = feed.ImageUrl ?? string.Empty,
                Posts = postList,
                Title = feed.Title ?? string.Empty,
                Url = url,
                Platform = platform
            };

            return feedModel;
        }
    }
}