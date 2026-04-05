using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlobalFlameMinistry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDonations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Module",
                table: "Donations");

            migrationBuilder.RenameColumn(
                name: "DonatedAt",
                table: "Donations",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "DonationType",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Donations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventTitle",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubaccountCode",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Donations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonationType",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "EventTitle",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "SubaccountCode",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Donations");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Donations",
                newName: "DonatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Module",
                table: "Donations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Ministry");
        }
    }
}
