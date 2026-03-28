using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaRocha.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailConfirmacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TokenConfirmacao",
                table: "Usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Marcar usuarios existentes como confirmados
            migrationBuilder.Sql("UPDATE \"Usuarios\" SET \"EmailConfirmado\" = true WHERE \"Excluido\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenConfirmacao",
                table: "Usuarios");
        }
    }
}
