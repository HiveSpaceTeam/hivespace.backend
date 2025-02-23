using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiveSpace.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileImageId",
                table: "categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "cart_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_skus_ProductId",
                table: "skus",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_skus_products_ProductId",
                table: "skus",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_skus_products_ProductId",
                table: "skus");

            migrationBuilder.DropIndex(
                name: "IX_skus_ProductId",
                table: "skus");

            migrationBuilder.DropColumn(
                name: "FileImageId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "cart_items");
        }
    }
}
