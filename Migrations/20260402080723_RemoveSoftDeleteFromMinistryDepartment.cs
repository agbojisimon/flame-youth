using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoftDeleteFromMinistryDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "MinistryDepartments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MinistryDepartments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "MinistryDepartments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MinistryDepartments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
