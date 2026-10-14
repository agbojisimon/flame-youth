using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogHeroFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FeaturedOnHeroAt",
                table: "BlogPosts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeaturedOnHero",
                table: "BlogPosts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsFeaturedOnHero_IsPublished_CreatedOn",
                table: "BlogPosts",
                columns: new[] { "IsFeaturedOnHero", "IsPublished", "CreatedOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_IsFeaturedOnHero_IsPublished_CreatedOn",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "FeaturedOnHeroAt",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "IsFeaturedOnHero",
                table: "BlogPosts");
        }
    }
}
