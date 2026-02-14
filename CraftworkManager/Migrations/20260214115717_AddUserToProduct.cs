using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftworkManager.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "StandardCost",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_userId",
                table: "Products",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_userId",
                table: "Products",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_userId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_userId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StandardCost",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Products");
        }
    }
}
