using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeHeroFeaturedEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FeaturedOn",
                table: "Events",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeaturedOnHome",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Events_IsFeaturedOnHome_StartDate",
                table: "Events",
                columns: new[] { "IsFeaturedOnHome", "StartDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_IsFeaturedOnHome_StartDate",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "FeaturedOn",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsFeaturedOnHome",
                table: "Events");
        }
    }
}
