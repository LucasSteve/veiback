using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeiCards.Infraestrutura.Persistencia.Migracoes
{
    /// <inheritdoc />
    public partial class RefinamentoPermissoesEColecao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "status_cartas_usuario");

            migrationBuilder.DropTable(
                name: "cartas");

            migrationBuilder.AddColumn<bool>(
                name: "inscricoes_abertas",
                table: "eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "cartas_colecionadas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    jogo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    carta_externa_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    raridade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    imagem_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tem = table.Column<bool>(type: "boolean", nullable: false),
                    quero = table.Column<bool>(type: "boolean", nullable: false),
                    favorito = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartas_colecionadas", x => x.id);
                    table.ForeignKey(
                        name: "FK_cartas_colecionadas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revogado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cartas_colecionadas_usuario_id_jogo",
                table: "cartas_colecionadas",
                columns: new[] { "usuario_id", "jogo" });

            migrationBuilder.CreateIndex(
                name: "IX_cartas_colecionadas_usuario_id_jogo_carta_externa_id",
                table: "cartas_colecionadas",
                columns: new[] { "usuario_id", "jogo", "carta_externa_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_usuario_id",
                table: "refresh_tokens",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cartas_colecionadas");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "inscricoes_abertas",
                table: "eventos");

            migrationBuilder.CreateTable(
                name: "cartas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    expansao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    imagem_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    jogo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    raridade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status_cartas_usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    carta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    favorito = table.Column<bool>(type: "boolean", nullable: false),
                    quero = table.Column<bool>(type: "boolean", nullable: false),
                    tem = table.Column<bool>(type: "boolean", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status_cartas_usuario", x => x.id);
                    table.ForeignKey(
                        name: "FK_status_cartas_usuario_cartas_carta_id",
                        column: x => x.carta_id,
                        principalTable: "cartas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_status_cartas_usuario_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cartas_jogo",
                table: "cartas",
                column: "jogo");

            migrationBuilder.CreateIndex(
                name: "IX_cartas_nome",
                table: "cartas",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "IX_cartas_raridade",
                table: "cartas",
                column: "raridade");

            migrationBuilder.CreateIndex(
                name: "IX_status_cartas_usuario_carta_id",
                table: "status_cartas_usuario",
                column: "carta_id");

            migrationBuilder.CreateIndex(
                name: "IX_status_cartas_usuario_usuario_id_carta_id",
                table: "status_cartas_usuario",
                columns: new[] { "usuario_id", "carta_id" },
                unique: true);
        }
    }
}
