using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class CourierRework_31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "X",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Y",
                table: "Users",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "X",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Users");
        }
    }
}
