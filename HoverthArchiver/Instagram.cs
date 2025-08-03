using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using Shared.Enums;
using Shared.Files;
using Shared.Models;

namespace HoverthArchiver;

public static class Instagram
{
    private static IInstaApi _instaApi;

    private static string GetExtension(string url)
    {
        return url.Split('?').First().TrimEnd('/').Split('/').Last().Split('.').Last();
    }

    public static async Task<Feed> AddInstagram(string url)
    {
        //var credname = "ttteeeeesssstt";
        //var cred = System.Text.Encoding.UTF8.GetString(
        //    System.Convert.FromBase64String("RXZ1VFhuZnB0WU9ETnBEQkwwaDZuWDRyUHY4V3ZrVGI="));

        var credname = System.Environment.GetEnvironmentVariable("INSTAGRAM_USERNAME");
        var cred = System.Environment.GetEnvironmentVariable("INSTAGRAM_PASSWORD");
        
        _instaApi ??= InstaApiBuilder.CreateBuilder()
            .SetUser(UserSessionData.ForUsername(credname).WithPassword(cred))
            .UseLogger(new DebugLogger(LogLevel.All))
            .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
            .Build();

        await _instaApi.LoginAsync(true);
        Thread.Sleep(1000);
        if (!_instaApi.IsUserAuthenticated) throw new Exception("User not authenticated");

        HoverthInput h = new HoverthInput(null, null);
        Console.WriteLine(_instaApi.IsUserAuthenticated);
        var username = url.Split('?').First().TrimEnd('/').Split('/').LastOrDefault();
        var userMedia =
            await _instaApi.UserProcessor.GetUserMediaAsync(username, PaginationParameters.MaxPagesToLoad(100));
        Thread.Sleep(1000);
        var user = (await _instaApi.UserProcessor.GetUserAsync(username)).Value;
        Thread.Sleep(1000);

        List<Post> posts = [];

        var imageUrl = "";
        var description = "";

        if (user != null)
        {
            var fullUser = (await _instaApi.UserProcessor.GetFullUserInfoAsync(user.Pk)).Value;

            if (!userMedia.Succeeded) throw new Exception("Could not get user media");

            foreach (var media in userMedia.Value)
            {
                Thread.Sleep(1000);
                List<Media> mediaItems = [];

                foreach (var item in media.Images)
                {
                    if (item.Uri == null) continue;
                    var filename = await h.DownloadFile(item.Uri);
                    var mediaItem = new Media()
                    {
                        Type = FileType.Image,
                        FileName = filename,
                    };
                    mediaItems.Add(mediaItem);
                }

                foreach (var item in media.Videos)
                {
                    if (item.Uri == null) continue;
                    var filename = await h.DownloadFile(item.Uri);
                    var mediaItem = new Media()
                    {
                        Type = FileType.Video,
                        FileName = filename,
                    };
                    mediaItems.Add(mediaItem);
                }

                Console.WriteLine("Processing item:" + media.Title + " - " + media.Code);
                var post = new Post
                {
                    Body = media.Caption.Text,
                    Title = media.Title,
                    Description = string.Empty,
                    PublishedAt = media.DeviceTimeStamp,
                    Media = mediaItems,
                };

                posts.Add(post);
            }

            if (user.ProfilePicUrl != null)
            {
                imageUrl = await h.DownloadFile(user.ProfilePicUrl);
            }

            if (fullUser != null)
            {
                description = fullUser.UserDetail.Biography ?? string.Empty;
            }
        }
        else
        {
            Console.WriteLine("Failed to get user!");
        }
        
        var feed = new Feed()
        {
            Title = "@" + username,
            Description = description,
            Url = url,
            Posts = posts,
            Platform = Platform.Instagram,
            ImageUrl = imageUrl ?? string.Empty,
        };

        return feed;
    }
}