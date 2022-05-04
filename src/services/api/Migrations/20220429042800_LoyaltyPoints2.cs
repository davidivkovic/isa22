using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class LoyaltyPoints2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tax");

            migrationBuilder.DropColumn(
                name: "BusinessOwnerPoints",
                table: "LoyaltyLevels");

            migrationBuilder.DropColumn(
                name: "CustomerPoints",
                table: "LoyaltyLevels");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "LoyaltyLevels",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Finances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxPercentage = table.Column<double>(type: "double precision", nullable: false),
                    CustomerPoints = table.Column<int>(type: "integer", nullable: false),
                    BusinessOwnerPoints = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finances", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Finances");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "LoyaltyLevels");

            migrationBuilder.AddColumn<int>(
                name: "BusinessOwnerPoints",
                table: "LoyaltyLevels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerPoints",
                table: "LoyaltyLevels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tax",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Percentage = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax", x => x.Id);
                });
        }
    }
}
