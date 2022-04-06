using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoyaltyLevels",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Threshold = table.Column<int>(type: "integer", nullable: false),
                    DiscountPercentage = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyLevels", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Tax",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Percentage = table.Column<double>(type: "double precision", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Address_Country = table.Column<string>(type: "text", nullable: true),
                    Address_PostalCode = table.Column<string>(type: "text", nullable: true),
                    Address_City = table.Column<string>(type: "text", nullable: true),
                    Address_Street = table.Column<string>(type: "text", nullable: true),
                    Address_Apartment = table.Column<string>(type: "text", nullable: true),
                    PricePerUnit_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    PricePerUnit_Currency = table.Column<string>(type: "text", nullable: true),
                    PricePerUnit_Symbol = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Images = table.Column<List<string>>(type: "text[]", nullable: true),
                    NumberOfReviews = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    Adventure_CancellationFeePercentage = table.Column<double>(type: "double precision", nullable: true),
                    Characteristics_Seats = table.Column<int>(type: "integer", nullable: true),
                    Characteristics_Length = table.Column<int>(type: "integer", nullable: true),
                    Characteristics_Engines = table.Column<int>(type: "integer", nullable: true),
                    Characteristics_BHP = table.Column<int>(type: "integer", nullable: true),
                    Characteristics_TopSpeed = table.Column<int>(type: "integer", nullable: true),
                    Equipment_Navigational = table.Column<List<string>>(type: "text[]", nullable: true),
                    Equipment_Additional = table.Column<List<string>>(type: "text[]", nullable: true),
                    CancellationFeePercentage = table.Column<double>(type: "double precision", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_Services",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Price_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Price_Currency = table.Column<string>(type: "text", nullable: true),
                    Price_Symbol = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_Services", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_Services_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    CabinId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Beds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => new { x.CabinId, x.Id });
                    table.ForeignKey(
                        name: "FK_Room_Businesses_CabinId",
                        column: x => x.CabinId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rule",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Allowed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rule", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Rule_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slot",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slot", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Slot_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Address_Country = table.Column<string>(type: "text", nullable: true),
                    Address_PostalCode = table.Column<string>(type: "text", nullable: true),
                    Address_City = table.Column<string>(type: "text", nullable: true),
                    Address_Street = table.Column<string>(type: "text", nullable: true),
                    Address_Apartment = table.Column<string>(type: "text", nullable: true),
                    Penalty_Points = table.Column<int>(type: "integer", nullable: true),
                    Penalty_Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoyaltyPoints = table.Column<int>(type: "integer", nullable: false),
                    LevelName = table.Column<string>(type: "text", nullable: true),
                    DeletionRequest_Reason = table.Column<string>(type: "text", nullable: true),
                    DeletionRequest_Answer = table.Column<string>(type: "text", nullable: true),
                    DeletionRequest_Approved = table.Column<bool>(type: "boolean", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_LoyaltyLevels_LevelName",
                        column: x => x.LevelName,
                        principalTable: "LoyaltyLevels",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Payment_Price_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Payment_Price_Currency = table.Column<string>(type: "text", nullable: true),
                    Payment_Price_Symbol = table.Column<string>(type: "text", nullable: true),
                    Payment_DiscountPercentage = table.Column<double>(type: "double precision", nullable: true),
                    Payment_TaxPercentage = table.Column<double>(type: "double precision", nullable: true),
                    Payment_Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DiscountPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Report_Reason = table.Column<string>(type: "text", nullable: true),
                    Report_IsApproved = table.Column<bool>(type: "boolean", nullable: true),
                    Report_Penalize = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Approved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Answer = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Answered = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Complaints_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations_Services",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Price_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Price_Currency = table.Column<string>(type: "text", nullable: true),
                    Price_Symbol = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations_Services", x => new { x.ReservationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Reservations_Services_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_OwnerId",
                table: "Businesses",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_UserId",
                table: "Businesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BusinessId",
                table: "Reservations",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_End",
                table: "Reservations",
                column: "End");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Start",
                table: "Reservations",
                column: "Start");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusinessId",
                table: "Reviews",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessId",
                table: "Users",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LevelName",
                table: "Users",
                column: "LevelName");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_OwnerId",
                table: "Businesses",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_UserId",
                table: "Businesses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_OwnerId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_UserId",
                table: "Businesses");

            migrationBuilder.DropTable(
                name: "Businesses_Services");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Reservations_Services");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Rule");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropTable(
                name: "Tax");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "LoyaltyLevels");
        }
    }
}
