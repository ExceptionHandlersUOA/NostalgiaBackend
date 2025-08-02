using HoverthArchiver;
using Shared.Database;

namespace NostalgiaBackend.Services
{
    public class DbInitializer(IServiceProvider serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<PostContext>();

            if (context.Feeds.Any())
            {
                return;
            }

            var feedUrls = new[]
            {
                "https://hoverth.com/feed.xml",
                "https://www.instagram.com/cristiano",
                "https://www.youtube.com/channel/UCRC6cNamj9tYAO6h_RXd5xA",
                "https://reddit.com/u/FraudKid"
            };

            foreach (var url in feedUrls)
            {
                try
                {
                    var feed = await HoverthInput.AddFeed(url);
                    if (feed != null)
                    {
                        await context.Feeds.AddAsync(feed, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add feed {url}: {ex.Message}");
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}