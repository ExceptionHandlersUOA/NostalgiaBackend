using Shared;
using CodeHollow.FeedReader;
using Shared.Enums;
using Shared.Models.Database;
using Feed = Shared.Models.Database.Feed;

namespace HoverthArchiver
{
    public class HoverthInput : IConsoleApplication
    {
        public void HandleInput(string[] args)
        {
            Console.WriteLine(string.Join(", ", args));

            if (args.Length < 1)
            {
                throw new Exception("No arguments");
            }

            if (args.Length >= 1)
            {
                var url = args.Skip(1).First();

                try
                {
                    switch (args.First())
                    {
                        case "yt":
                            YouTube(url).GetAwaiter().GetResult();
                            break;
                        case "rss":
                            RssAsync(url).GetAwaiter().GetResult();
                            break;
                        case "reddit":
                            Reddit(url).GetAwaiter().GetResult();
                            break;
                        case "test":
                            Test([.. args.Skip(1)]);
                            break;
                    }
                }
                catch (HttpRequestException e)
                {
                    // Timed out or similar errors
                    Console.WriteLine(e);
                }
                catch (System.Xml.XmlException e)
                {
                    // we've probably accidentally gotten an HTML page
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("No command");
            }
        }

        private static void Test(string[] _)
        {
            HtmlParser parser = new();
            parser.GetImages("<p><img src=\"url thing is here yadda\"/>Besides, the fantasy of an unlived life is always preposterously rosy. When you fantasise about the career you might have had, you never stop to consider the horrific chairlift injury that caused both of your legs to be amputated, or the car accident that killed your sister and mother who were on their way to pick you up from the airport. There is no such thing as a perfect speedrun of life. There’s always something to be regretted. I’m sure many of the peers you are comparing yourself to feel envious of aspects of your life. Even those who “have everything” never really have everything, and Instagram is the worst possible tool to use as an objective yardstick.</p>");
        }

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

        private static async Task<Feed> RssAsync(string url, Platform platform = Platform.RSS)
        {
            HtmlParser parser = new();
            var feed = await FeedReader.ReadAsync(url);

            Console.WriteLine("Feed Title: " + feed.Title);
            Console.WriteLine("Feed Description: " + feed.Description);
            Console.WriteLine("Feed Image: " + feed.ImageUrl);

            foreach (var item in feed.Items)
            {
                Console.WriteLine(item.Title + " - " + item.Link);
                // Console.WriteLine(item.Content);
                // item.Content is HTML - need to parse, extract images + videos and text

                Console.WriteLine(parser.FlattenText(item.Content));
                Console.WriteLine(string.Join(", ", parser.GetImages(item.Content)));
            }

            var feedModel = new Feed()
            {
                Description = feed.Description ?? string.Empty,
                FeedId = 0,
                ImageUrl = feed.ImageUrl ?? string.Empty,
                Posts = [.. feed.Items.Select(item => new Post
                {
                    Title = item.Title ?? string.Empty,
                })],
                Title = feed.Title ?? string.Empty,
                Url = url,
                Platform = platform
            };

            return feedModel;
        }
    }
}