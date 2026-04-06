using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaRocha.Migrations
{
    /// <inheritdoc />
    public partial class claudeClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Mensalistas_TenantId_Status",
                table: "Mensalistas",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_TenantId_BarbeiroId_DataHora",
                table: "Agendamentos",
                columns: new[] { "TenantId", "BarbeiroId", "DataHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_TenantId_Status",
                table: "Agendamentos",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_TenantId_UsuarioId",
                table: "Agendamentos",
                columns: new[] { "TenantId", "UsuarioId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Mensalistas_TenantId_Status",
                table: "Mensalistas");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_TenantId_BarbeiroId_DataHora",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_TenantId_Status",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_TenantId_UsuarioId",
                table: "Agendamentos");
        }
    }
}
