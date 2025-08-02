using HoverthArchiver;
using Shared.Database;

namespace NostalgiaBackend
{
    public static class DbInitializer
    {
        public static async void Initialize(PostContext context)
        {
            if (context.Feeds.Any())
            {
                return;
            }

            await HoverthInput.AddFeed("https://hoverth.com/feed.xml");
            await HoverthInput.AddFeed("https://www.instagram.com/cristiano");
            await HoverthInput.AddFeed("https://www.youtube.com/channel/UCRC6cNamj9tYAO6h_RXd5xA");
            await HoverthInput.AddFeed("https://reddit.com/u/FraudKid");

            context.SaveChanges();
        }
    }
}