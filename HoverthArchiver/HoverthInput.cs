using CodeHollow.FeedReader;
using FeroxArchiver;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Files;
using Shared.Models;
using YoutubeDLSharp;
using Feed = Shared.Models.Feed;

namespace HoverthArchiver
{
    public class HoverthInput(ILogger<HoverthInput> logger, FeroxInput ferox) : IHostedService
    {
        private readonly ILogger<HoverthInput> logger = logger;
        private readonly YoutubeDL _youtubeDL = new();

        private readonly HttpClient _httpClient = new()
        {
            Timeout = new TimeSpan(0, 5, 0) // 5 minute timeout,
        };

        private readonly FeroxInput ferox = ferox;

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
            string channelId = url.Split('/').Last().Split('?').First();
            var rssUrl = "https://www.youtube.com/feeds/videos.xml?channel_id=" + channelId;
            var youtubeRss = await RssAsync(rssUrl, Platform.YouTube);

            foreach (var post in youtubeRss.Posts)
            {
                var source = post.SourceUrl;

                var video = await _youtubeDL.RunVideoDownload(source);
                var metadata = await _youtubeDL.RunVideoDataFetch(source);

                if (video.Success)
                {
                    using var stream = File.OpenRead(video.Data);

                    var thumbnailUrl = metadata.Data.Thumbnail;
                    logger.LogInformation("Downloading thumbnail: {thumbnail}", thumbnailUrl);

                    var stream2 = await _httpClient.GetStreamAsync(thumbnailUrl);
                    var thumbnail = await StaticFiles.AddFileToSystem(stream2, "jpg");

                    var media = new Media()
                    {
                        Type = FileType.Video,
                        FileName = await StaticFiles.AddFileToSystem(stream, metadata.Data.Extension),
                        ThumbnailFileName = thumbnail,
                    };

                    post.Media.Add(media);
                }
            }

            return youtubeRss;
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
            var feed = await FeedReader.ReadAsync(url, true, "Mozilla/5.0 (X11; Linux x86_64; rv:140.0) Gecko/20100101 Firefox/140.0");

            logger.LogInformation("Feed Title: {Title}", feed.Title);
            logger.LogInformation("Feed Description: {Description}", feed.Description);
            logger.LogInformation("Feed Image: {ImageUrl}", feed.ImageUrl);

            List<Post> postList = [];

            foreach (var item in feed.Items)
            {
                logger.LogInformation("Processing item: {Title} - {Link}", item.Title, item.Link);

                var textContent = parser.FlattenText(item.Content);
                var imageUrls = parser.GetImages(item.Content);
                imageUrls.Concat(parser.GetImages(item.Description));
                var videoUrls = parser.GetVideos(item.Content);
                videoUrls.Concat(parser.GetVideos(item.Description));
                var documentUrls = parser.GetDocuments(item.Content);
                documentUrls.Concat(parser.GetDocuments(item.Description));

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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Utils.DownloadYtDlp();
            await Utils.DownloadFFmpeg();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}