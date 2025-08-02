using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class FeedPlatform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "Platform",
                table: "Feeds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                table: "Feeds");

            migrationBuilder.AddColumn<int>(
                name: "Platform",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
