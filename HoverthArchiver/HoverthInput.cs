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
                if (args.First() == "yt") 
                {
                    YouTube(args.Skip(1).ToArray());
                } 
                else if (args.First() == "rss") 
                {
                    RSS(args.Skip(1).ToArray());
                }
            }
            else 
            {
                Console.WriteLine("No command");
            }
        }

        void YouTube(string[] args)
        {
            // change yt channel link into https://www.youtube.com/feeds/videos.xml?channel_id=UCRC6cNamj9tYAO6h_RXd5xA format
            // https://www.youtube.com/channel/UCRC6cNamj9tYAO6h_RXd5xA

            if (args.Length >= 1) 
            {
                var url = args.First();
                // Console.WriteLine(url);
                string channel_id = url.Split('/').Last().Split('?').First();
                // Console.WriteLine(channel_id);
                var rss_url = "https://www.youtube.com/feeds/videos.xml?channel_id=" + channel_id;
                // Console.WriteLine(rss_url);
                RSS([rss_url]); // return
            }
        }

        async void RSS(string[] args)
        {
            if (args.Length >= 1) 
            {
                try 
                {
                    var feed = await FeedReader.ReadAsync(args.First());

                    Console.WriteLine("Feed Title: " + feed.Title);
                    Console.WriteLine("Feed Description: " + feed.Description);
                    Console.WriteLine("Feed Image: " + feed.ImageUrl);
    
                    foreach(var item in feed.Items)
                    {
                        Console.WriteLine(item.Title + " - " + item.Link);
                    }
                    // return 200
                } catch (System.Net.Http.HttpRequestException e) 
                {
                    Console.WriteLine(e);
                    // throw 500 
                }
            }
        }
    }
}
