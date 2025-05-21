using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Order15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentWeight",
                table: "Users",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentWeight",
                table: "Users");
        }
    }
}
