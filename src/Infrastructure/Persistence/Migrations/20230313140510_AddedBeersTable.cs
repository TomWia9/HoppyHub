using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedBeersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Beers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AlcoholByVolume",
                table: "Beers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Blg",
                table: "Beers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brewery",
                table: "Beers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Beers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Beers",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ibu",
                table: "Beers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Plato",
                table: "Beers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SpecificGravity",
                table: "Beers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "Beers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholByVolume",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "Blg",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "Brewery",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "Ibu",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "Plato",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "SpecificGravity",
                table: "Beers");

            migrationBuilder.DropColumn(
                name: "Style",
                table: "Beers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Beers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
