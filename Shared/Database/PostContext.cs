using Microsoft.EntityFrameworkCore;
using Shared.Models.Database;

namespace Shared.Database
{
    public class PostContext : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Media> Media { get; set; }

        public string DbPath { get; }

        public PostContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "posts.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
