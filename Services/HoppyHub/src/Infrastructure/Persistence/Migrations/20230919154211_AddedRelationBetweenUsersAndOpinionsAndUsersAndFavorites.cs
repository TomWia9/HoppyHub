using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddedRelationBetweenUsersAndOpinionsAndUsersAndFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Opinions_CreatedBy",
                table: "Opinions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_CreatedBy",
                table: "Favorites",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Users_CreatedBy",
                table: "Favorites",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Opinions_Users_CreatedBy",
                table: "Opinions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Users_CreatedBy",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Opinions_Users_CreatedBy",
                table: "Opinions");

            migrationBuilder.DropIndex(
                name: "IX_Opinions_CreatedBy",
                table: "Opinions");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_CreatedBy",
                table: "Favorites");
        }
    }
}
