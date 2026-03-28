using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaRocha.Migrations
{
    /// <inheritdoc />
    public partial class mensalistaupgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PeriodoTrabalho",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BarbeiroId",
                table: "Mensalistas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Horario",
                table: "Mensalistas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicoId",
                table: "Mensalistas",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodoTrabalho",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "BarbeiroId",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "Horario",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "Mensalistas");
        }
    }
}
