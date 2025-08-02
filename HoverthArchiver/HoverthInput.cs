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
                }
            }
            else
            {
                Console.WriteLine("No command");
            }
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
                var feed = await FeedReader.ReadAsync(args.First());

                Console.WriteLine("Feed Title: " + feed.Title);
                Console.WriteLine("Feed Description: " + feed.Description);
                Console.WriteLine("Feed Image: " + feed.ImageUrl);

                foreach (var item in feed.Items)
                {
                    Console.WriteLine(item.Title + " - " + item.Link);
                }
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