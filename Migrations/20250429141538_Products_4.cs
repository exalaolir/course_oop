using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Products_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mark",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mark",
                table: "Products");
        }
    }
}
