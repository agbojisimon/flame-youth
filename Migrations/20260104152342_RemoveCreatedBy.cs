using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace g_flame_youth.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_CreatedById",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_CreatedById",
                table: "Announcements");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Announcements",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CreatedById",
                table: "Announcements",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_CreatedById",
                table: "Announcements",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
