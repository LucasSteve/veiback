using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeiCards.Infraestrutura.Persistencia.Migracoes
{
    /// <inheritdoc />
    public partial class MigracaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cartas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    expansao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    raridade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    jogo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    imagem_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "eventos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    horario = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    local = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    organizador = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    formato = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    capacidade = table.Column<int>(type: "integer", nullable: true),
                    imagem_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nome_exibicao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    senha_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    papel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inscricoes_eventos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    evento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_inscricao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inscricoes_eventos", x => x.id);
                    table.ForeignKey(
                        name: "FK_inscricoes_eventos_eventos_evento_id",
                        column: x => x.evento_id,
                        principalTable: "eventos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inscricoes_eventos_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "noticias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    resumo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    conteudo = table.Column<string>(type: "text", nullable: true),
                    categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    autor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    data_publicacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tempo_leitura_minutos = table.Column<int>(type: "integer", nullable: true),
                    imagem_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_noticias", x => x.id);
                    table.ForeignKey(
                        name: "FK_noticias_usuarios_autor_id",
                        column: x => x.autor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "status_cartas_usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    carta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tem = table.Column<bool>(type: "boolean", nullable: false),
                    quero = table.Column<bool>(type: "boolean", nullable: false),
                    favorito = table.Column<bool>(type: "boolean", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "IX_eventos_cidade",
                table: "eventos",
                column: "cidade");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_data",
                table: "eventos",
                column: "data");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_tipo",
                table: "eventos",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "IX_inscricoes_eventos_evento_id_usuario_id",
                table: "inscricoes_eventos",
                columns: new[] { "evento_id", "usuario_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inscricoes_eventos_usuario_id",
                table: "inscricoes_eventos",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_noticias_autor_id",
                table: "noticias",
                column: "autor_id");

            migrationBuilder.CreateIndex(
                name: "IX_noticias_categoria",
                table: "noticias",
                column: "categoria");

            migrationBuilder.CreateIndex(
                name: "IX_status_cartas_usuario_carta_id",
                table: "status_cartas_usuario",
                column: "carta_id");

            migrationBuilder.CreateIndex(
                name: "IX_status_cartas_usuario_usuario_id_carta_id",
                table: "status_cartas_usuario",
                columns: new[] { "usuario_id", "carta_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_nome_usuario",
                table: "usuarios",
                column: "nome_usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inscricoes_eventos");

            migrationBuilder.DropTable(
                name: "noticias");

            migrationBuilder.DropTable(
                name: "status_cartas_usuario");

            migrationBuilder.DropTable(
                name: "eventos");

            migrationBuilder.DropTable(
                name: "cartas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
