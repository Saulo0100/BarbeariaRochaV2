using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BarbeariaRocha.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoSiteEDominioAutorizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DominiosAutorizados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Dominio = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DominiosAutorizados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracaoSite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DominioId = table.Column<int>(type: "integer", nullable: false),
                    NomeSite = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Logo = table.Column<byte[]>(type: "bytea", nullable: true),
                    CorPrimaria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CorSecundaria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CorFundo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CorTexto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CorAcento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoSite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracaoSite_DominiosAutorizados_DominioId",
                        column: x => x.DominioId,
                        principalTable: "DominiosAutorizados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracaoSite_DominioId",
                table: "ConfiguracaoSite",
                column: "DominioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DominiosAutorizados_Dominio",
                table: "DominiosAutorizados",
                column: "Dominio",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoSite");

            migrationBuilder.DropTable(
                name: "DominiosAutorizados");
        }
    }
}
