using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiveSpace.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceAttributeValueToAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_product_attribute_values_AttributeId",
                table: "product_attribute_values",
                column: "AttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_product_attribute_values_attributes_AttributeId",
                table: "product_attribute_values",
                column: "AttributeId",
                principalTable: "attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_attribute_values_attributes_AttributeId",
                table: "product_attribute_values");

            migrationBuilder.DropIndex(
                name: "IX_product_attribute_values_AttributeId",
                table: "product_attribute_values");
        }
    }
}
