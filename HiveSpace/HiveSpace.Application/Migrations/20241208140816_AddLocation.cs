using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HiveSpace.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "administrative_regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CodeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CodeNameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrative_regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "administrative_units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullNameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ShortNameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CodeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CodeNameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrative_units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "provinces",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullNameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CodeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AdministrativeUnitId = table.Column<int>(type: "integer", nullable: false),
                    AdministrativeRegionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provinces", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "districts",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullNameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CodeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProvinceCode = table.Column<string>(type: "character varying(20)", nullable: false),
                    AdministrativeUnitId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.Code);
                    table.ForeignKey(
                        name: "FK_districts_administrative_units_AdministrativeUnitId",
                        column: x => x.AdministrativeUnitId,
                        principalTable: "administrative_units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_districts_provinces_ProvinceCode",
                        column: x => x.ProvinceCode,
                        principalTable: "provinces",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wards",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullNameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CodeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DistrictCode = table.Column<string>(type: "character varying(20)", nullable: false),
                    AdministrativeUnitId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wards", x => x.Code);
                    table.ForeignKey(
                        name: "FK_wards_administrative_units_AdministrativeUnitId",
                        column: x => x.AdministrativeUnitId,
                        principalTable: "administrative_units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_wards_districts_DistrictCode",
                        column: x => x.DistrictCode,
                        principalTable: "districts",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_districts_AdministrativeUnitId",
                table: "districts",
                column: "AdministrativeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_districts_ProvinceCode",
                table: "districts",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_wards_AdministrativeUnitId",
                table: "wards",
                column: "AdministrativeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_wards_DistrictCode",
                table: "wards",
                column: "DistrictCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrative_regions");

            migrationBuilder.DropTable(
                name: "wards");

            migrationBuilder.DropTable(
                name: "districts");

            migrationBuilder.DropTable(
                name: "administrative_units");

            migrationBuilder.DropTable(
                name: "provinces");
        }
    }
}
