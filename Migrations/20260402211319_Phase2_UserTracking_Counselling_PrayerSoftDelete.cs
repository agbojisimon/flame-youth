using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_UserTracking_Counselling_PrayerSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "PrayerRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PrayerRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "EventRegistrations",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubaccountCode",
                table: "Donations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventTitle",
                table: "Donations",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CounsellingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Topic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredContact = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Email"),
                    AssignedTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AssignedToEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounsellingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CounsellingRequests_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_AppUserId",
                table: "EventRegistrations",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CounsellingRequests_AppUserId",
                table: "CounsellingRequests",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrations_AspNetUsers_AppUserId",
                table: "EventRegistrations",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrations_AspNetUsers_AppUserId",
                table: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "CounsellingRequests");

            migrationBuilder.DropIndex(
                name: "IX_EventRegistrations_AppUserId",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "PrayerRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PrayerRequests");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "EventRegistrations");

            migrationBuilder.AlterColumn<string>(
                name: "SubaccountCode",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventTitle",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
