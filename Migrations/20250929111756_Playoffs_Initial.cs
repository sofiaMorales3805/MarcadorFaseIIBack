using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarcadorFaseIIApi.Migrations
{
    /// <inheritdoc />
    public partial class Playoffs_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Torneos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Temporada = table.Column<int>(type: "int", nullable: false),
                    BestOf = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torneos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TorneoId = table.Column<int>(type: "int", nullable: false),
                    Ronda = table.Column<int>(type: "int", nullable: false),
                    SeedA = table.Column<int>(type: "int", nullable: false),
                    SeedB = table.Column<int>(type: "int", nullable: false),
                    EquipoAId = table.Column<int>(type: "int", nullable: false),
                    EquipoBId = table.Column<int>(type: "int", nullable: false),
                    BestOf = table.Column<int>(type: "int", nullable: false),
                    WinsA = table.Column<int>(type: "int", nullable: false),
                    WinsB = table.Column<int>(type: "int", nullable: false),
                    Cerrada = table.Column<bool>(type: "bit", nullable: false),
                    GanadorEquipoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_Torneos_TorneoId",
                        column: x => x.TorneoId,
                        principalTable: "Torneos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TorneoId = table.Column<int>(type: "int", nullable: false),
                    SeriePlayoffId = table.Column<int>(type: "int", nullable: false),
                    GameNumber = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EquipoLocalId = table.Column<int>(type: "int", nullable: false),
                    EquipoVisitanteId = table.Column<int>(type: "int", nullable: false),
                    MarcadorLocal = table.Column<int>(type: "int", nullable: true),
                    MarcadorVisitante = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidos_Series_SeriePlayoffId",
                        column: x => x.SeriePlayoffId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartidosJugadores",
                columns: table => new
                {
                    PartidoId = table.Column<int>(type: "int", nullable: false),
                    EquipoId = table.Column<int>(type: "int", nullable: false),
                    JugadorId = table.Column<int>(type: "int", nullable: false),
                    Titular = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidosJugadores", x => new { x.PartidoId, x.EquipoId, x.JugadorId });
                    table.ForeignKey(
                        name: "FK_PartidosJugadores_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_SeriePlayoffId_GameNumber",
                table: "Partidos",
                columns: new[] { "SeriePlayoffId", "GameNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Series_TorneoId_Ronda",
                table: "Series",
                columns: new[] { "TorneoId", "Ronda" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartidosJugadores");

            migrationBuilder.DropTable(
                name: "Partidos");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Torneos");
        }
    }
}
