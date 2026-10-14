using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedToHeroSlides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeaturedOrder",
                table: "HeroSlides",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "HeroSlides",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_HeroSlides_MediaType_IsFeatured_IsActive",
                table: "HeroSlides",
                columns: new[] { "MediaType", "IsFeatured", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HeroSlides_MediaType_IsFeatured_IsActive",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "FeaturedOrder",
                table: "HeroSlides");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "HeroSlides");
        }
    }
}
