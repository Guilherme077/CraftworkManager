using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftworkManager.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDeadline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeadlineOn",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeadlineOn",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderItems");
        }
    }
}
