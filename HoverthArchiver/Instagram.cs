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
            .UseLogger(new DebugLogger(LogLevel.All))
            .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
            .Build();
        
        var username = url.Split('?').First().TrimEnd('/').Split('/').Last();
        var userMedia = await _instaApi.UserProcessor.GetUserMediaAsync(username, PaginationParameters.MaxPagesToLoad(100));
        var user = (await _instaApi.UserProcessor.GetUserAsync(username)).Value;
        var fullUser = (await _instaApi.UserProcessor.GetFullUserInfoAsync(user.Pk)).Value;
        
        if (!userMedia.Succeeded) throw new Exception("Could not get user media");

        List<Post> posts = new List<Post>();
        
        foreach (var media in userMedia.Value)
        {
            List<Media> mediaItems = new List<Media>();

            foreach (var item in media.Images)
            {
                Stream s = new MemoryStream(item.ImageBytes);
                var mediaItem = new Media()
                {
                    Type = FileType.Image,
                    FileName = await StaticFiles.AddFileToSystem(s, GetExtension(item.Uri)),
                };
                mediaItems.Add(mediaItem);
            }
            
            foreach (var item in media.Videos)
            {
                Stream s = new MemoryStream(item.VideoBytes);
                var mediaItem = new Media()
                {
                    Type = FileType.Video,
                    FileName = await StaticFiles.AddFileToSystem(s, GetExtension(item.Uri)),
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

        HoverthInput h = new HoverthInput(null, null);
        var imageUrl = await h.DownloadFile(user.ProfilePicUrl);
        
        return new Feed()
        {
            Title = "@" +  username,
            Description = fullUser.UserDetail.Biography,
            Url = url,
            Posts = posts,
            Platform = Platform.Instagram,
            ImageUrl = imageUrl,
        };
    }
}