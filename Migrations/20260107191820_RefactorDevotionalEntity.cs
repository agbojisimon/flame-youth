using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace g_flame_youth.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDevotionalEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Devotionals",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Devotionals",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DevotionalDate",
                table: "Devotionals",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Devotionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Devotionals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Devotionals_DevotionalDate",
                table: "Devotionals",
                column: "DevotionalDate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devotionals_DevotionalDate",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "DevotionalDate",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Devotionals");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Devotionals",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Devotionals",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);
        }
    }
}
