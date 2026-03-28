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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoHorario");
        }
    }
}
