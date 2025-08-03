using HoverthArchiver;
using Shared.Database;

namespace NostalgiaBackend.Services
{
    public class DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger, HoverthInput hoverth) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Downloading deps...");

            await hoverth.DownloadDeps();

            logger.LogInformation("Starting database initialization...");

            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<PostContext>();

            if (context.Feeds.Any())
            {
                return;
            }

            var feedUrls = new[]
            {
                "https://github.com/FeroxFoxxo",
                "https://www.hoverth.net/index.xml",
                "https://www.instagram.com/cristiano",
                "https://www.youtube.com/channel/UCFZ1dO0j7fmX5P1OoDRGMjg",
                "https://reddit.com/u/FraudKid",
                "https://cdn.jwz.org/blog/feed/",
                "https://blog.ncase.me/feed.xml"
            };

            foreach (var url in feedUrls)
            {
                try
                {
                    var feed = await hoverth.AddFeed(url);
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