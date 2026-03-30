using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftworkManager.Migrations
{
    /// <inheritdoc />
    public partial class AddModificationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModificationType",
                table: "ShipmentStatusHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModificationType",
                table: "OrderStatusHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModificationType",
                table: "ShipmentStatusHistory");

            migrationBuilder.DropColumn(
                name: "ModificationType",
                table: "OrderStatusHistory");
        }
    }
}
