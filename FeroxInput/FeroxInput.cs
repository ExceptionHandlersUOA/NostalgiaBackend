using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Files;
using Shared.Models;

namespace FeroxArchiver
{
    public class FeroxInput(ILogger<FeroxInput> logger)
    {
        public async Task<Feed> GitHub(string url)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "FeroxArchiver");
            
            var uri = new Uri(url);
            var username = uri.Segments.LastOrDefault()?.Trim('/');
            
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Invalid GitHub URL");
            
            var userResponse = await httpClient.GetStringAsync($"https://api.github.com/users/{username}");
            var userJson = System.Text.Json.JsonDocument.Parse(userResponse);
            var userRoot = userJson.RootElement;
            
            var reposResponse = await httpClient.GetStringAsync($"https://api.github.com/users/{username}/repos?sort=updated");
            var reposJson = System.Text.Json.JsonDocument.Parse(reposResponse);
            
            var posts = new List<Post>();

            foreach (var repo in reposJson.RootElement.EnumerateArray())
            {
                var repoName = repo.GetProperty("name").GetString() ?? "";
                logger.LogInformation("Downloading repo: {repoName}", repoName);

                var defaultBranch = repo.GetProperty("default_branch").GetString() ?? "main";
                
                var archiveUrl = $"https://github.com/{username}/{repoName}/archive/refs/heads/{defaultBranch}.zip";
                var downloadedFilePath = await StaticFiles.AddFileToSystem(await httpClient.GetStreamAsync(archiveUrl), "zip");
                
                var media = new List<Media>();
                if (!string.IsNullOrEmpty(downloadedFilePath))
                {
                    media.Add(new Media
                    {
                        Type = FileType.Document,
                        FileName = downloadedFilePath
                    });
                }
                logger.LogInformation("Downloaded media");

                posts.Add(new Post
                {
                    Title = repoName,
                    Description = repo.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                    SourceUrl = repo.GetProperty("html_url").GetString() ?? "",
                    PublishedAt = repo.GetProperty("created_at").GetDateTime(),
                    LastUpdated = repo.GetProperty("updated_at").GetDateTime(),
                    Categories = [],
                    Media = media
                });
            }

            var imageUrl = userRoot.GetProperty("avatar_url").GetString();
            var imageName = await StaticFiles.AddFileToSystem(await httpClient.GetStreamAsync(imageUrl), "jpg");

            return new Feed
            {
                Title = userRoot.TryGetProperty("name", out var name) ? name.GetString() ?? username : username,
                Description = userRoot.TryGetProperty("bio", out var bio) ? bio.GetString() ?? "" : "",
                ImageUrl = imageName,
                Url = url,
                Posts = posts,
                Platform = Platform.GitHub
            };
        }
    }
}
