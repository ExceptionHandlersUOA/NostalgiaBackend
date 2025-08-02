using Shared.Enums;
using Shared.Models.Web;

namespace Shared
{
    public class SampleData
    {
        public static List<WebPost> GetSamplePosts() =>
        [
            new() {
                PostId = 1,
                Title = "Exploring the New Horizon of .NET 8",
                Description = "An in-depth look at performance improvements and C# 12 features shipping with .NET 8.",
                SourceUrl = "https://devblogs.microsoft.com/dotnet/announcing-net-8",
                PublishedAt = new DateTime(2025, 7, 15, 10, 30, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 7, 16, 14, 45, 0, DateTimeKind.Utc),
                Media = [],
                Platform = Platform.LinkedIn
            },

            new() {
                PostId = 2,
                Title = "Summer Landscape Photography",
                Description = "A curated set of high-resolution shots from our recent trip to the Swiss Alps.",
                SourceUrl = "https://example.com/alps-gallery",
                PublishedAt = new DateTime(2025, 6, 20, 8, 0, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 6, 20, 8, 0, 0, DateTimeKind.Utc),
                Media =
                [
                    new WebMedia
                    {
                        MediaId = 6,
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/alps-01.jpg"
                    },
                    new WebMedia
                    {
                        MediaId = 5,
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/alps-02.jpg"
                    },
                    new WebMedia
                    {
                        MediaId = 4,
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/alps-03.jpg"
                    }
                ],
                Platform = Platform.Instagram
            },

            new() {
                PostId = 3,
                Title = "Introducing Our Product Demo",
                Description = "Watch our quick walkthrough of the latest features in action. Perfect for users on the go!",
                SourceUrl = "https://youtube.com/watch?v=abcdef12345",
                PublishedAt = new DateTime(2025, 7, 1, 12, 0, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 7, 2, 9, 15, 0, DateTimeKind.Utc),
                Media =
                [
                    new WebMedia
                    {
                        MediaId = 2,
                        Type = FileType.Video,
                        FileUrl = "https://cdn.example.com/videos/demo.mp4"
                    },
                    new WebMedia
                    {
                        MediaId = 3,
                        Type = FileType.Image,
                        FileUrl = "https://cdn.example.com/images/demo-thumbnail.jpg"
                    }
                ],
                Platform = Platform.Twitter
            },
            new() {
                PostId = 4,
                Title = "Q2 Financial Report (PDF)",
                Description = "Download our detailed Q2 report, including charts, analyses, and executive summary.",
                SourceUrl = "https://example.com/reports/q2-2025.pdf",
                PublishedAt = new DateTime(2025, 7, 30, 16, 0, 0, DateTimeKind.Utc),
                LastUpdated = new DateTime(2025, 7, 30, 16, 0, 0, DateTimeKind.Utc),
                Media =
                [
                    new WebMedia
                    {
                        MediaId = 1,
                        Type = FileType.Document,
                        FileUrl = "https://cdn.example.com/docs/Q2-2025-Report.pdf"
                    }
                ],
                Platform = Platform.Facebook
            }
        ];

    }
}
