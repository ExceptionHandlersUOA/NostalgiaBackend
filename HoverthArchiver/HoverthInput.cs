using System.IO.Compression;
using Shared;
using CodeHollow.FeedReader;

namespace HoverthArchiver
{
    public class HoverthInput : IConsoleApplication
    {
        public void HandleInput(string[] args)
        {
            Console.WriteLine(string.Join(", ", args));
            if (args.Length >= 1)
            {
                switch (args.First())
                {
                    case "yt":
                        YouTube(args.Skip(1).ToArray());
                        break;
                    case "rss":
                        RSS(args.Skip(1).ToArray());
                        break;
                    case "reddit":
                        Reddit(args.Skip(1).ToArray());
                        break;
                    case "test":
                        Test(args.Skip(1).ToArray());
                        break;
                }
            }
            else
            {
                Console.WriteLine("No command");
            }
        }

        void Test(string[] args)
        {
            HtmlParser parser = new HtmlParser();
            parser.GetImages("<p><img src=\"url thing is here yadda\"/>Besides, the fantasy of an unlived life is always preposterously rosy. When you fantasise about the career you might have had, you never stop to consider the horrific chairlift injury that caused both of your legs to be amputated, or the car accident that killed your sister and mother who were on their way to pick you up from the airport. There is no such thing as a perfect speedrun of life. There’s always something to be regretted. I’m sure many of the peers you are comparing yourself to feel envious of aspects of your life. Even those who “have everything” never really have everything, and Instagram is the worst possible tool to use as an objective yardstick.</p>");
        }
        
        void Reddit(string[] args)
        {
            if (args.Length < 1)
            {
                throw new Exception("No arguments");
            }

            var url = args.First();
            Console.WriteLine(url);
            var rss_url = url + ".rss";
            //var rss_url = FeedReader.GetFeedUrlsFromUrl(url).First().Url;
            Console.WriteLine(rss_url);
            RSS([rss_url]);
        }

        void YouTube(string[] args)
        {
            // change yt channel link into https://www.youtube.com/feeds/videos.xml?channel_id=UCRC6cNamj9tYAO6h_RXd5xA format
            // https://www.youtube.com/channel/UCRC6cNamj9tYAO6h_RXd5xA

            if (args.Length < 1)
            {
                throw new Exception("No arguments");
            }

            var url = args.First();
            // Console.WriteLine(url);
            string channel_id = url.Split('/').Last().Split('?').First();
            // Console.WriteLine(channel_id);
            var rss_url = "https://www.youtube.com/feeds/videos.xml?channel_id=" + channel_id;
            // Console.WriteLine(rss_url);
            RSS([rss_url]); // return
        }

        async void RSS(string[] args)
        {
            if (args.Length < 1)
            {
                throw new Exception("No arguments");
            }

            try
            {
                HtmlParser parser = new HtmlParser();
                var url = args.First();
                var feed = await FeedReader.ReadAsync(url);

                Console.WriteLine("Feed Title: " + feed.Title);
                Console.WriteLine("Feed Description: " + feed.Description);
                Console.WriteLine("Feed Image: " + feed.ImageUrl);

                foreach (var item in feed.Items)
                {
                    Console.WriteLine(item.Title + " - " + item.Link);
                    Console.WriteLine(item.Content);
                    // item.Content is HTML - need to parse, extract images + videos and text

                    parser.FlattenText(item.Content);
                }

                Shared.Models.WebFeedModel webFeed = new Shared.Models.WebFeedModel();
                webFeed.Title = feed.Title;
                webFeed.Description = feed.Description;
                webFeed.ImageUrl = feed.ImageUrl;
                webFeed.Url = url;
                // return 200
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                // Timed out or similar errors
                Console.WriteLine(e);
                // throw 500 
            }
            catch (System.Xml.XmlException e)
            {
                // we've probably accidentally gotten an HTML page
                Console.WriteLine(e);
                // throw 500
            }
        }
    }
}