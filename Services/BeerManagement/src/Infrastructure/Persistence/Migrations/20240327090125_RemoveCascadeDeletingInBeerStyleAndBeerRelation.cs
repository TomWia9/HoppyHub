using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCascadeDeletingInBeerStyleAndBeerRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beers_BeerStyles_BeerStyleId",
                table: "Beers");

            migrationBuilder.AlterColumn<Guid>(
                name: "BeerStyleId",
                table: "Beers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Beers_BeerStyles_BeerStyleId",
                table: "Beers",
                column: "BeerStyleId",
                principalTable: "BeerStyles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beers_BeerStyles_BeerStyleId",
                table: "Beers");

            migrationBuilder.AlterColumn<Guid>(
                name: "BeerStyleId",
                table: "Beers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Beers_BeerStyles_BeerStyleId",
                table: "Beers",
                column: "BeerStyleId",
                principalTable: "BeerStyles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
