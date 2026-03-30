using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftworkManager.Migrations
{
    /// <inheritdoc />
    public partial class FixStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentStatusHistory_Orders_OrderId",
                table: "ShipmentStatusHistory");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "ShipmentStatusHistory",
                newName: "ShipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentStatusHistory_OrderId",
                table: "ShipmentStatusHistory",
                newName: "IX_ShipmentStatusHistory_ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentStatusHistory_Shipments_ShipmentId",
                table: "ShipmentStatusHistory",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentStatusHistory_Shipments_ShipmentId",
                table: "ShipmentStatusHistory");

            migrationBuilder.RenameColumn(
                name: "ShipmentId",
                table: "ShipmentStatusHistory",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentStatusHistory_ShipmentId",
                table: "ShipmentStatusHistory",
                newName: "IX_ShipmentStatusHistory_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentStatusHistory_Orders_OrderId",
                table: "ShipmentStatusHistory",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
