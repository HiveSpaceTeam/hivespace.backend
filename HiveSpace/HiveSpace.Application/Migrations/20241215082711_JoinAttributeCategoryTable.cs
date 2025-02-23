using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiveSpace.Application.Migrations
{
    /// <inheritdoc />
    public partial class JoinAttributeCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attribute_categories",
                columns: table => new
                {
                    AttributesId = table.Column<int>(type: "integer", nullable: false),
                    CategoriesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attribute_categories", x => new { x.AttributesId, x.CategoriesId });
                    table.ForeignKey(
                        name: "FK_attribute_categories_attributes_AttributesId",
                        column: x => x.AttributesId,
                        principalTable: "attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attribute_categories_categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attribute_categories_CategoriesId",
                table: "attribute_categories",
                column: "CategoriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attribute_categories");
        }
    }
}
