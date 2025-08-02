using HoverthArchiver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using System.ComponentModel.DataAnnotations;

namespace NostalgiaBackend.Controllers
{
    public class AddFeedUrlRequest
    {
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/feed")]
    public class FeedController(PostContext context, HoverthInput hoverth) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody, Required] AddFeedUrlRequest request)
        {
            if (string.IsNullOrEmpty(request.Url))
            {
                return BadRequest("URL is required");
            }

            if (context.Feeds.FirstOrDefault(f => f.Url == request.Url) != null)
            {
                return BadRequest("Feed already exists");
            }

            var feed = await hoverth.AddFeed(request.Url);

            context.Feeds.Add(feed);

            await context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpDelete("{feedId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int feedId)
        {
            var feed = await context.Feeds
                .Include(f => f.Posts)
                .ThenInclude(p => p.Media)
                .FirstOrDefaultAsync(f => f.FeedId == feedId);

            if (feed == null)
            {
                return BadRequest("Feed not found");
            }

            context.Feeds.Remove(feed);

            foreach (var post in feed.Posts)
            {
                context.Posts.Remove(post);

                foreach (var media in post.Media)
                {
                    context.Media.Remove(media);
                }
            }

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{feedId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int feedId)
        {
            var feed = await context.Feeds
                .Include(f => f.Posts)
                .ThenInclude(p => p.Media)
                .FirstOrDefaultAsync(f => f.FeedId == feedId);

            if (feed == null)
            {
                return BadRequest("Feed not found");
            }

            var newFeed = await hoverth.AddFeed(feed.Url);

            if (newFeed == null)
            {
                return BadRequest("Failed to update feed");
            }

            feed.Title = newFeed.Title;
            feed.Description = newFeed.Description;
            feed.ImageUrl = newFeed.ImageUrl;
            feed.Url = newFeed.Url;
            feed.Platform = newFeed.Platform;

            var existingPostsLookup = feed.Posts.ToDictionary(p => p.SourceUrl, p => p);

            foreach (var newPost in newFeed.Posts)
            {
                if (existingPostsLookup.TryGetValue(newPost.SourceUrl, out var existingPost))
                {
                    existingPost.Title = newPost.Title;
                    existingPost.Description = newPost.Description;
                    existingPost.Body = newPost.Body;
                    existingPost.LastUpdated = newPost.LastUpdated;
                    existingPost.PublishedAt = newPost.PublishedAt;
                    existingPost.Categories = newPost.Categories;

                    var existingMediaLookup = existingPost.Media.ToDictionary(m => m.FileName, m => m);
                    
                    foreach (var newMedia in newPost.Media)
                    {
                        if (existingMediaLookup.TryGetValue(newMedia.FileName, out var existingMedia))
                        {
                            existingMedia.Type = newMedia.Type;
                        }
                        else
                        {
                            context.Media.Add(newMedia);
                            await context.SaveChangesAsync();

                            existingPost.Media.Add(newMedia);
                        }
                    }
                }
                else
                {
                    context.Posts.Add(newPost);
                    await context.SaveChangesAsync();

                    feed.Posts.Add(newPost);
                }
            }

            context.Feeds.Update(feed);
            await context.SaveChangesAsync();
            
            return Ok();
        }
    }
}