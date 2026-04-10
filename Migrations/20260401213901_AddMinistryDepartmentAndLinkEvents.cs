using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMinistryDepartmentAndLinkEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinistryId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MinistryDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LeaderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LeaderTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LeaderImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinistryDepartments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_MinistryId",
                table: "Events",
                column: "MinistryId");

            migrationBuilder.CreateIndex(
                name: "IX_MinistryDepartments_Slug",
                table: "MinistryDepartments",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_MinistryDepartments_MinistryId",
                table: "Events",
                column: "MinistryId",
                principalTable: "MinistryDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_MinistryDepartments_MinistryId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "MinistryDepartments");

            migrationBuilder.DropIndex(
                name: "IX_Events_MinistryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MinistryId",
                table: "Events");
        }
    }
}
