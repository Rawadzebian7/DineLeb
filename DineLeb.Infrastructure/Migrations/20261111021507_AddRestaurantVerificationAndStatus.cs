using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DineLeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantVerificationAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommercialRegisterNumber",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Restaurants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Restaurants",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialRegisterNumber",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Restaurants");
        }
    }
}
