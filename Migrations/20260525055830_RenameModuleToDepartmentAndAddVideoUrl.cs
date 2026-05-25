using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameModuleToDepartmentAndAddVideoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Module",
                table: "BlogPosts",
                newName: "Department");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Testimonies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Testimonies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "BlogPosts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Testimonies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Testimonies");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "BlogPosts",
                newName: "Module");
        }
    }
}
