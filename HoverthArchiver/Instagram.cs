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
        _instaApi ??= InstaApiBuilder.CreateBuilder()
            .SetUser(UserSessionData.ForUsername("ttteeeeesssstt").WithPassword(System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("RXZ1VFhuZnB0WU9ETnBEQkwwaDZuWDRyUHY4V3ZrVGI="))))
            .UseLogger(new DebugLogger(LogLevel.All))
            .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
            .Build();

        await _instaApi.LoginAsync(true);
        if (!_instaApi.IsUserAuthenticated) throw new Exception("User not authenticated");

        HoverthInput h = new HoverthInput(null, null);
        Console.WriteLine(_instaApi.IsUserAuthenticated);
        var username = url.Split('?').First().TrimEnd('/').Split('/').LastOrDefault();
        Console.WriteLine(username);
        Console.WriteLine(_instaApi.UserProcessor.ToString());
        var userMedia =
            await _instaApi.UserProcessor.GetUserMediaAsync(username, PaginationParameters.MaxPagesToLoad(100));
        var user = (await _instaApi.UserProcessor.GetUserAsync(username)).Value;
        var fullUser = (await _instaApi.UserProcessor.GetFullUserInfoAsync(user.Pk)).Value;

        if (!userMedia.Succeeded) throw new Exception("Could not get user media");

        List<Post> posts = new List<Post>();

        foreach (var media in userMedia.Value)
        {
            List<Media> mediaItems = new List<Media>();

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

        var imageUrl = "";
        if (user.ProfilePicUrl != null)
        {
            imageUrl = await h.DownloadFile(user.ProfilePicUrl);
        }

        var description = "";
        if (fullUser != null)
        {
            description = fullUser.UserDetail.Biography ?? string.Empty;
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