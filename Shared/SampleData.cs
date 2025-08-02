using Shared.Enums;
using Shared.Models;

namespace Shared
{
    public class SampleData
    {
        public static List<WebPostModel> GetSamplePosts() =>
        [
            new() {
                Id = Guid.NewGuid().ToString(),
                Title = "Exploring the New Horizon of .NET 8",
                Description = "An in-depth look at performance improvements and C# 12 features shipping with .NET 8.",
                SourceUrl = "https://devblogs.microsoft.com/dotnet/announcing-net-8",
                PublishedAt = new DateTime(2025, 7, 15, 10, 30, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 7, 16, 14, 45, 0, DateTimeKind.Utc),
                Media = [],
                Platform = Platform.LinkedIn
            },

            new() {
                Id = Guid.NewGuid().ToString(),
                Title = "Summer Landscape Photography",
                Description = "A curated set of high-resolution shots from our recent trip to the Swiss Alps.",
                SourceUrl = "https://example.com/alps-gallery",
                PublishedAt = new DateTime(2025, 6, 20, 8, 0, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 6, 20, 8, 0, 0, DateTimeKind.Utc),
                Media =
                [
                    new WebMediaModel
                    {
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/alps-01.jpg"
                    },
                    new WebMediaModel
                    {
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/alps-02.jpg"
                    },
                    new WebMediaModel
                    {
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/alps-03.jpg"
                    }
                ],
                Platform = Platform.Instagram
            },

            new() {
                Id = Guid.NewGuid().ToString(),
                Title = "Introducing Our Product Demo",
                Description = "Watch our quick walkthrough of the latest features in action. Perfect for users on the go!",
                SourceUrl = "https://youtube.com/watch?v=abcdef12345",
                PublishedAt = new DateTime(2025, 7, 1, 12, 0, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 7, 2, 9, 15, 0, DateTimeKind.Utc),
                Media =
                [
                    new WebMediaModel
                    {
                        Type = FileType.Video,
                        FileUrl = "https://cdn.example.com/videos/demo.mp4"
                    },
                    new WebMediaModel
                    {
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/demo-thumbnail.jpg"
                    }
                ],
                Platform = Platform.Twitter
            },
            new() {
                Id = Guid.NewGuid().ToString(),
                Title = "Q2 Financial Report (PDF)",
                Description = "Download our detailed Q2 report, including charts, analyses, and executive summary.",
                SourceUrl = "https://example.com/reports/q2-2025.pdf",
                PublishedAt = new DateTime(2025, 7, 30, 16, 0, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 7, 30, 16, 0, 0, DateTimeKind.Utc),
                Media =
                [
                    new WebMediaModel
                    {
                        Type = FileType.Document,
                        FileUrl = "https://cdn.example.com/docs/Q2-2025-Report.pdf"
                    }
                ],
                Platform = Platform.Facebook
            }
        ];

    }
}
