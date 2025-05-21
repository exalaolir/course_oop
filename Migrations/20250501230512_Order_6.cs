using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Order_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "X",
                table: "Orders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Y",
                table: "Orders",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "X",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Orders");
        }
    }
}
