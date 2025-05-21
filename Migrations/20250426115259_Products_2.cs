using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Products_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategotyId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategotyId",
                table: "Products",
                column: "CategotyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Category_CategotyId",
                table: "Products",
                column: "CategotyId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Category_CategotyId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategotyId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategotyId",
                table: "Products");
        }
    }
}
