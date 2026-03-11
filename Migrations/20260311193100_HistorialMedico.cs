using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlqoMishi.Migrations
{
    /// <inheritdoc />
    public partial class HistorialMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialesMedicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MascotaId = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnoId = table.Column<int>(type: "INTEGER", nullable: false),
                    VeterinarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Diagnostico = table.Column<string>(type: "TEXT", nullable: false),
                    Tratamiento = table.Column<string>(type: "TEXT", nullable: true),
                    Medicamentos = table.Column<string>(type: "TEXT", nullable: true),
                    Peso = table.Column<decimal>(type: "TEXT", nullable: true),
                    Temperatura = table.Column<decimal>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesMedicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesMedicos_Empleados_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialesMedicos_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialesMedicos_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesMedicos_MascotaId",
                table: "HistorialesMedicos",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesMedicos_TurnoId",
                table: "HistorialesMedicos",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesMedicos_VeterinarioId",
                table: "HistorialesMedicos",
                column: "VeterinarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialesMedicos");
        }
    }
}
