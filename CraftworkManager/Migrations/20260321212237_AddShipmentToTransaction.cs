using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftworkManager.Migrations
{
    /// <inheritdoc />
    public partial class AddShipmentToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ShipmentId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ShipmentId",
                table: "Transactions",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Shipments_ShipmentId",
                table: "Transactions",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Shipments_ShipmentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ShipmentId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ShipmentId",
                table: "Transactions");
        }
    }
}
