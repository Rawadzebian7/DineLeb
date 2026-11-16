using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DineLeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncRestaurantSubscriptionAndReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribed",
                table: "Restaurants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReferenceNumber",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionExpiresAt",
                table: "Restaurants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionPlan",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribed",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "PaymentReferenceNumber",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SubscriptionExpiresAt",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "Restaurants");
        }
    }
}
