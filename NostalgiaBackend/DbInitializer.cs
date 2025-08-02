using Shared.Database;
using Shared.Enums;
using Shared.Models;

namespace NostalgiaBackend
{
    public static class DbInitializer
    {
        public static void Initialize(PostContext context)
        {
            if (context.Feeds.Any())
            {
                return;
            }

            var feeds = new Feed[]
            {
                new() {
                    Title = "Daily Inspiration",
                    Description = "Motivational content to start your day",
                    ImageUrl = "https://example.com/inspiration.jpg",
                    Url = "https://feeds.example.com/inspiration",
                    Posts = [
                        new() {
                            Title = "Morning Coffee Vibes",
                            Description = "Starting the day with a perfect cup of coffee",
                            SourceUrl = "https://example.com/coffee",
                            LastUpdated = DateTime.Now.AddDays(-1),
                            PublishedAt = DateTime.Now.AddDays(-2),
                            Media =
                            [
                                new Media { Type = FileType.Image, FileName = "coffee_morning.jpg" }
                            ]
                        }, new() {
                            Title = "Weekend Adventures",
                            Description = "Exploring the great outdoors this weekend",
                            SourceUrl = "https://example.com/adventure",
                            LastUpdated = DateTime.Now.AddDays(-3),
                            PublishedAt = DateTime.Now.AddDays(-4),
                            Media =
                            [
                                new Media { Type = FileType.Image, FileName = "hiking_trail.jpg" },
                                new Media { Type = FileType.Video, FileName = "nature_walk.mp4" }
                            ]
                        }
                    ],
                    Platform = Platform.Instagram,
                },
                new() {
                    Title = "Tech Updates",
                    Description = "Latest news and updates from the tech world",
                    ImageUrl = "https://example.com/tech.jpg",
                    Url = "https://feeds.example.com/tech",
                    Posts = [
                        new() {
                            Title = "Tech Talk Tuesday",
                            Description = "Discussing the latest in software development",
                            SourceUrl = "https://example.com/tech",
                            LastUpdated = DateTime.Now.AddDays(-5),
                            PublishedAt = DateTime.Now.AddDays(-6)
                        }
                    ],
                    Platform = Platform.LinkedIn
                },
            };

            context.Feeds.AddRange(feeds);
            context.SaveChanges();
        }
    }
}