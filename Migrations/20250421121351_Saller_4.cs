using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Saller_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Category_CategoryId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Shop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SallerId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shop_Users_SallerId",
                        column: x => x.SallerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Shop_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_ShopId",
                table: "Image",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Shop_SallerId",
                table: "Shop",
                column: "SallerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Category_CategoryId",
                table: "Users",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Category_CategoryId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Shop");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Category_CategoryId",
                table: "Users",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
