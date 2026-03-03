using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaRocha.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiSlotScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Servico: novas colunas para serviços com 2 etapas
            migrationBuilder.AddColumn<bool>(
                name: "RequerDuasEtapas",
                table: "Servicos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IntervaloMinimoHoras",
                table: "Servicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DescricaoEtapa1",
                table: "Servicos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescricaoEtapa2",
                table: "Servicos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Agendamento: novas colunas para vincular etapas
            migrationBuilder.AddColumn<int>(
                name: "AgendamentoPrincipalId",
                table: "Agendamentos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescricaoEtapa",
                table: "Agendamentos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_AgendamentoPrincipalId",
                table: "Agendamentos",
                column: "AgendamentoPrincipalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Agendamentos_AgendamentoPrincipalId",
                table: "Agendamentos",
                column: "AgendamentoPrincipalId",
                principalTable: "Agendamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Agendamentos_AgendamentoPrincipalId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_AgendamentoPrincipalId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "AgendamentoPrincipalId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "DescricaoEtapa",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "RequerDuasEtapas",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "IntervaloMinimoHoras",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "DescricaoEtapa1",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "DescricaoEtapa2",
                table: "Servicos");
        }
    }
}
