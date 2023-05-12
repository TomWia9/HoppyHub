using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedPrimaryBeerStylesAndBeerStylesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Style",
                table: "Beers");

            migrationBuilder.AddColumn<Guid>(
                name: "BeerStyleId",
                table: "Beers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PrimaryBeerStyles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 40, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryBeerStyles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeerStyles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrimaryBeerStyleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 40, nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeerStyles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeerStyles_PrimaryBeerStyles_PrimaryBeerStyleId",
                        column: x => x.PrimaryBeerStyleId,
                        principalTable: "PrimaryBeerStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beers_BeerStyleId",
                table: "Beers",
                column: "BeerStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerStyles_PrimaryBeerStyleId",
                table: "BeerStyles",
                column: "PrimaryBeerStyleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Beers_BeerStyles_BeerStyleId",
                table: "Beers",
                column: "BeerStyleId",
                principalTable: "BeerStyles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beers_BeerStyles_BeerStyleId",
                table: "Beers");

            migrationBuilder.DropTable(
                name: "BeerStyles");

            migrationBuilder.DropTable(
                name: "PrimaryBeerStyles");

            migrationBuilder.DropIndex(
                name: "IX_Beers_BeerStyleId",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "BeerStyleId",
                table: "Beers");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "Beers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
