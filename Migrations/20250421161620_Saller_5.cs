using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace course_oop.Migrations
{
    /// <inheritdoc />
    public partial class Saller_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Shop_ShopId",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_Shop_Users_SallerId",
                table: "Shop");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shop",
                table: "Shop");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Image",
                table: "Image");

            migrationBuilder.RenameTable(
                name: "Shop",
                newName: "Shops");

            migrationBuilder.RenameTable(
                name: "Image",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_Shop_SallerId",
                table: "Shops",
                newName: "IX_Shops_SallerId");

            migrationBuilder.RenameIndex(
                name: "IX_Image_ShopId",
                table: "Images",
                newName: "IX_Images_ShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shops",
                table: "Shops",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Shops_ShopId",
                table: "Images",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Users_SallerId",
                table: "Shops",
                column: "SallerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Shops_ShopId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Users_SallerId",
                table: "Shops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shops",
                table: "Shops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Shops",
                newName: "Shop");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "Image");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_SallerId",
                table: "Shop",
                newName: "IX_Shop_SallerId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ShopId",
                table: "Image",
                newName: "IX_Image_ShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shop",
                table: "Shop",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Image",
                table: "Image",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Shop_ShopId",
                table: "Image",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shop_Users_SallerId",
                table: "Shop",
                column: "SallerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
