using Microsoft.EntityFrameworkCore;
using Shared.Models.Database;

namespace NostalgiaBackend.Database
{
    public class PostContext : DbContext
    {
        public DbSet<Post> Blogs { get; set; }
        public DbSet<Media> Posts { get; set; }

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
