using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BarbeariaRocha.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoHorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracaoHorario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiaSemana = table.Column<int>(type: "integer", nullable: false),
                    Aberto = table.Column<bool>(type: "boolean", nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    AlmocoInicio = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    AlmocoFim = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    HoraFim = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    IntervaloMinutos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoHorario", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoHorario_DiaSemana",
                table: "ConfiguracaoHorario",
                column: "DiaSemana",
                unique: true);

            // Seed: valores equivalentes ao que era hardcoded no HelperGenerico
            // Domingo (0) — fechado
            migrationBuilder.InsertData("ConfiguracaoHorario",
                ["DiaSemana", "Aberto", "HoraInicio", "AlmocoInicio", "AlmocoFim", "HoraFim", "IntervaloMinutos"],
                new object[] { 0, false, null, null, null, null, 40 });

            // Segunda-feira (1) — somente tarde, sem almoço
            migrationBuilder.InsertData("ConfiguracaoHorario",
                ["DiaSemana", "Aberto", "HoraInicio", "AlmocoInicio", "AlmocoFim", "HoraFim", "IntervaloMinutos"],
                new object[] { 1, true, new TimeOnly(13, 20), null, null, new TimeOnly(20, 0), 40 });

            // Terça-feira (2)
            migrationBuilder.InsertData("ConfiguracaoHorario",
                ["DiaSemana", "Aberto", "HoraInicio", "AlmocoInicio", "AlmocoFim", "HoraFim", "IntervaloMinutos"],
                new object[] { 2, true, new TimeOnly(10, 0), new TimeOnly(11, 20), new TimeOnly(13, 20), new TimeOnly(20, 0), 40 });

            // Quarta-feira (3)
            migrationBuilder.InsertData("ConfiguracaoHorario",
                ["DiaSemana", "Aberto", "HoraInicio", "AlmocoInicio", "AlmocoFim", "HoraFim", "IntervaloMinutos"],
                new object[] { 3, true, new TimeOnly(10, 0), new TimeOnly(11, 20), new TimeOnly(13, 20), new TimeOnly(20, 0), 40 });

            // Quinta-feira (4)
            migrationBuilder.InsertData("ConfiguracaoHorario",
                ["DiaSemana", "Aberto", "HoraInicio", "AlmocoInicio", "AlmocoFim", "HoraFim", "IntervaloMinutos"],
                new object[] { 4, true, new TimeOnly(10, 0), new TimeOnly(11, 20), new TimeOnly(13, 20), new TimeOnly(20, 0), 40 });

            // Sexta-feira (5)
            migrationBuilder.InsertData("ConfiguracaoHorario",
                ["DiaSemana", "Aberto", "HoraInicio", "AlmocoInicio", "AlmocoFim", "HoraFim", "IntervaloMinutos"],
                new object[] { 5, true, new TimeOnly(10, 0), new TimeOnly(11, 20), new TimeOnly(13, 20), new TimeOnly(20, 0), 40 });

            // Sábado (6)
            migrationBuilder.InsertData("ConfiguracaoHorario",
                ["DiaSemana", "Aberto", "HoraInicio", "AlmocoInicio", "AlmocoFim", "HoraFim", "IntervaloMinutos"],
                new object[] { 6, true, new TimeOnly(9, 0), new TimeOnly(12, 20), new TimeOnly(13, 20), new TimeOnly(17, 20), 40 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoHorario");
        }
    }
}
