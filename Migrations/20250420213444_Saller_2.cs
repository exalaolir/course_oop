using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Saller_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CategoryId",
                table: "Users",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Category_CategoryId",
                table: "Users",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Category_CategoryId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CategoryId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
