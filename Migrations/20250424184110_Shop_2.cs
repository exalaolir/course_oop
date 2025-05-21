using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Shop_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "X",
                table: "Shops",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Y",
                table: "Shops",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "X",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Shops");
        }
    }
}
