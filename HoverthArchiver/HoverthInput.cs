using CodeHollow.FeedReader;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Feed = Shared.Models.Feed;
using Shared.Models;
using Shared.Files;
using FeroxArchiver;

namespace HoverthArchiver
{
    public class HoverthInput(ILogger<HoverthInput> logger, FeroxInput ferox)
    {
        private readonly ILogger<HoverthInput> logger = logger;

        private readonly HttpClient _httpClient = new()
        {
            Timeout = new TimeSpan(0, 5, 0) // 5 minute timeout,
        };

        public async Task<Feed> AddFeed(string url, Platform platform = Platform.RSS)
        {
            if (url.Contains("reddit.com"))
            {
                return await Reddit(url);
            }

            if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            {
                return await YouTube(url);
            }

            if (url.Contains("github.com"))
            {
                return await ferox.GitHub(url);
            }

            if (url.Contains("instagram.com"))
            {
                return await Instagram.AddInstagram(url);
            }

            return await RssAsync(url, platform);
        }

        private async Task<Feed> Reddit(string url)
        {
            logger.LogInformation("Processing Reddit URL: {Url}", url);
            var rssUrl = url + ".rss";
            logger.LogInformation("Generated RSS URL: {RssUrl}", rssUrl);
            return await RssAsync(rssUrl, Platform.Reddit);
        }

        private async Task<Feed> YouTube(string url)
        {
            // change yt channel link into https://www.youtube.com/feeds/videos.xml?channel_id=UCRC6cNamj9tYAO6h_RXd5xA format
            // https://www.youtube.com/channel/UCRC6cNamj9tYAO6h_RXd5xA

            string channelId = url.Split('/').Last().Split('?').First();
            var rssUrl = "https://www.youtube.com/feeds/videos.xml?channel_id=" + channelId;
            return await RssAsync(rssUrl, Platform.YouTube);
        }

        public async Task<string> DownloadFile(string url)
        {
            var remoteFilename = url.Split('/').Last().Split('?').First();
            var extension = remoteFilename.Split('.').Last();

            var stream = await _httpClient.GetStreamAsync(url);

            var file = await StaticFiles.AddFileToSystem(stream, extension);

            return file;
        }

        private async Task<Feed> RssAsync(string url, Platform platform = Platform.RSS)
        {
            HtmlParser parser = new();
            var feed = await FeedReader.ReadAsync(url);

            logger.LogInformation("Feed Title: {Title}", feed.Title);
            logger.LogInformation("Feed Description: {Description}", feed.Description);
            logger.LogInformation("Feed Image: {ImageUrl}", feed.ImageUrl);

            List<Post> postList = [];

            foreach (var item in feed.Items)
            {
                logger.LogInformation("Processing item: {Title} - {Link}", item.Title, item.Link);

                var textContent = parser.FlattenText(item.Content);
                var imageUrls = parser.GetImages(item.Content);
                var videoUrls = parser.GetVideos(item.Content);
                var documentUrls = parser.GetDocuments(item.Content);

                List<Media> mediaList = [];

                foreach (var image in imageUrls)
                {
                    var media = new Media()
                    {
                        Type = FileType.Image,
                        FileName = await DownloadFile(image),
                    };
                    mediaList.Add(media);
                }

                foreach (var video in videoUrls)
                {
                    var media = new Media()
                    {
                        Type = FileType.Video,
                        FileName = await DownloadFile(video),
                    };
                    mediaList.Add(media);
                }

                foreach (var document in documentUrls)
                {
                    var media = new Media()
                    {
                        Type = FileType.Document,
                        FileName = await DownloadFile(document),
                    };
                    mediaList.Add(media);
                }

                var post = new Post()
                {
                    Description = parser.FlattenText(item.Description) ?? string.Empty,
                    Title = item.Title ?? string.Empty,
                    SourceUrl = item.Link ?? string.Empty,
                    PublishedAt = item.PublishingDate ?? DateTime.MinValue,
                    Categories = [],
                    Favourited = false,
                    LastUpdated = DateTime.Now,
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