using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class LoyaltyPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessOwnerPoints",
                table: "LoyaltyLevels");

            migrationBuilder.DropColumn(
                name: "CustomerPoints",
                table: "LoyaltyLevels");
        }
    }
}
