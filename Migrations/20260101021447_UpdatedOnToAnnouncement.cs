using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace g_flame_youth.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOnToAnnouncement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Announcements",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Announcements");
        }
    }
}
