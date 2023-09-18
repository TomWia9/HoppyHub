using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class RemovedPrimaryBeerStylesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BeerStyles_PrimaryBeerStyles_PrimaryBeerStyleId",
                table: "BeerStyles");

            migrationBuilder.DropTable(
                name: "PrimaryBeerStyles");

            migrationBuilder.DropIndex(
                name: "IX_BeerStyles_PrimaryBeerStyleId",
                table: "BeerStyles");

            migrationBuilder.DropColumn(
                name: "PrimaryBeerStyleId",
                table: "BeerStyles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryBeerStyleId",
                table: "BeerStyles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PrimaryBeerStyles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 40, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 40, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryBeerStyles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeerStyles_PrimaryBeerStyleId",
                table: "BeerStyles",
                column: "PrimaryBeerStyleId");

            migrationBuilder.AddForeignKey(
                name: "FK_BeerStyles_PrimaryBeerStyles_PrimaryBeerStyleId",
                table: "BeerStyles",
                column: "PrimaryBeerStyleId",
                principalTable: "PrimaryBeerStyles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
