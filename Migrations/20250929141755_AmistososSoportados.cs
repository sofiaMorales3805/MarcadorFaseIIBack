using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarcadorFaseIIApi.Migrations
{
    /// <inheritdoc />
    public partial class AmistososSoportados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Partidos_SeriePlayoffId_GameNumber",
                table: "Partidos");

            migrationBuilder.AlterColumn<int>(
                name: "TorneoId",
                table: "Partidos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SeriePlayoffId",
                table: "Partidos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GameNumber",
                table: "Partidos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_SeriePlayoffId_GameNumber",
                table: "Partidos",
                columns: new[] { "SeriePlayoffId", "GameNumber" },
                unique: true,
                filter: "[SeriePlayoffId] IS NOT NULL AND [GameNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Partidos_SeriePlayoffId_GameNumber",
                table: "Partidos");

            migrationBuilder.AlterColumn<int>(
                name: "TorneoId",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SeriePlayoffId",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GameNumber",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_SeriePlayoffId_GameNumber",
                table: "Partidos",
                columns: new[] { "SeriePlayoffId", "GameNumber" },
                unique: true);
        }
    }
}
