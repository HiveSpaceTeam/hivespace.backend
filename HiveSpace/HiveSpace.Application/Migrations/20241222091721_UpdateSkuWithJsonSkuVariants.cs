using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HiveSpace.Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSkuWithJsonSkuVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProducVarianttName",
                table: "skus");

            migrationBuilder.AddColumn<string>(
                name: "SkuVariants",
                table: "skus",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkuVariants",
                table: "skus");

            migrationBuilder.AddColumn<string>(
                name: "ProducVarianttName",
                table: "skus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
