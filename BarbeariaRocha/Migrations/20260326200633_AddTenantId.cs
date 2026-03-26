using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaRocha.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Numero",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Mensalistas_Numero",
                table: "Mensalistas");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Servicos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Mensalistas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "MensalistaCortes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Excecoes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ConfiguracaoHorario",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ConfiguracaoBarbearia",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CodigoConfirmacao",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Agendamentos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AgendamentoAdicional",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Adicional",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_TenantId_Numero",
                table: "Usuarios",
                columns: new[] { "TenantId", "Numero" },
                unique: true,
                filter: "\"Excluido\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Mensalistas_TenantId_Numero",
                table: "Mensalistas",
                columns: new[] { "TenantId", "Numero" },
                unique: true,
                filter: "\"Status\" = 'Ativo'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_TenantId_Numero",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Mensalistas_TenantId_Numero",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MensalistaCortes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Excecoes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ConfiguracaoHorario");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ConfiguracaoBarbearia");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CodigoConfirmacao");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AgendamentoAdicional");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Adicional");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Numero",
                table: "Usuarios",
                column: "Numero",
                unique: true,
                filter: "\"Excluido\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Mensalistas_Numero",
                table: "Mensalistas",
                column: "Numero",
                unique: true,
                filter: "\"Status\" = 'Ativo'");
        }
    }
}
