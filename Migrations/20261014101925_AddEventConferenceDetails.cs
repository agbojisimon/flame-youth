using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEventConferenceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnchorScripture",
                table: "Events",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnchorScriptureReference",
                table: "Events",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConferenceTheme",
                table: "Events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnchorScripture",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "AnchorScriptureReference",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ConferenceTheme",
                table: "Events");
        }
    }
}
