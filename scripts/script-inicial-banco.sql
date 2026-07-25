CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE TABLE cartas (
        id uuid NOT NULL,
        nome character varying(200) NOT NULL,
        numero character varying(20),
        expansao character varying(100),
        raridade character varying(50),
        jogo character varying(50),
        imagem_url character varying(500),
        CONSTRAINT "PK_cartas" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE TABLE eventos (
        id uuid NOT NULL,
        nome character varying(200) NOT NULL,
        descricao text,
        data timestamp with time zone NOT NULL,
        horario character varying(10),
        local character varying(200),
        cidade character varying(100),
        organizador character varying(150),
        formato character varying(100),
        tipo character varying(20) NOT NULL,
        capacidade integer,
        imagem_url character varying(500),
        CONSTRAINT "PK_eventos" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE TABLE usuarios (
        id uuid NOT NULL,
        nome_usuario character varying(50) NOT NULL,
        email character varying(200) NOT NULL,
        nome_exibicao character varying(100) NOT NULL,
        senha_hash character varying(200) NOT NULL,
        papel character varying(20) NOT NULL,
        criado_em timestamp with time zone NOT NULL,
        CONSTRAINT "PK_usuarios" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE TABLE inscricoes_eventos (
        id uuid NOT NULL,
        evento_id uuid NOT NULL,
        usuario_id uuid NOT NULL,
        data_inscricao timestamp with time zone NOT NULL,
        CONSTRAINT "PK_inscricoes_eventos" PRIMARY KEY (id),
        CONSTRAINT "FK_inscricoes_eventos_eventos_evento_id" FOREIGN KEY (evento_id) REFERENCES eventos (id) ON DELETE CASCADE,
        CONSTRAINT "FK_inscricoes_eventos_usuarios_usuario_id" FOREIGN KEY (usuario_id) REFERENCES usuarios (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE TABLE noticias (
        id uuid NOT NULL,
        titulo character varying(200) NOT NULL,
        resumo character varying(500),
        conteudo text,
        categoria character varying(50),
        autor_id uuid,
        data_publicacao timestamp with time zone NOT NULL,
        tempo_leitura_minutos integer,
        imagem_url character varying(500),
        CONSTRAINT "PK_noticias" PRIMARY KEY (id),
        CONSTRAINT "FK_noticias_usuarios_autor_id" FOREIGN KEY (autor_id) REFERENCES usuarios (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE TABLE status_cartas_usuario (
        id uuid NOT NULL,
        usuario_id uuid NOT NULL,
        carta_id uuid NOT NULL,
        tem boolean NOT NULL,
        quero boolean NOT NULL,
        favorito boolean NOT NULL,
        atualizado_em timestamp with time zone NOT NULL,
        CONSTRAINT "PK_status_cartas_usuario" PRIMARY KEY (id),
        CONSTRAINT "FK_status_cartas_usuario_cartas_carta_id" FOREIGN KEY (carta_id) REFERENCES cartas (id) ON DELETE CASCADE,
        CONSTRAINT "FK_status_cartas_usuario_usuarios_usuario_id" FOREIGN KEY (usuario_id) REFERENCES usuarios (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_cartas_jogo" ON cartas (jogo);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_cartas_nome" ON cartas (nome);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_cartas_raridade" ON cartas (raridade);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_eventos_cidade" ON eventos (cidade);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_eventos_data" ON eventos (data);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_eventos_tipo" ON eventos (tipo);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE UNIQUE INDEX "IX_inscricoes_eventos_evento_id_usuario_id" ON inscricoes_eventos (evento_id, usuario_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_inscricoes_eventos_usuario_id" ON inscricoes_eventos (usuario_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_noticias_autor_id" ON noticias (autor_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_noticias_categoria" ON noticias (categoria);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE INDEX "IX_status_cartas_usuario_carta_id" ON status_cartas_usuario (carta_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE UNIQUE INDEX "IX_status_cartas_usuario_usuario_id_carta_id" ON status_cartas_usuario (usuario_id, carta_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE UNIQUE INDEX "IX_usuarios_email" ON usuarios (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    CREATE UNIQUE INDEX "IX_usuarios_nome_usuario" ON usuarios (nome_usuario);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260724232633_MigracaoInicial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260724232633_MigracaoInicial', '10.0.10');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    DROP TABLE status_cartas_usuario;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    DROP TABLE cartas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    ALTER TABLE eventos ADD inscricoes_abertas boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    CREATE TABLE cartas_colecionadas (
        id uuid NOT NULL,
        usuario_id uuid NOT NULL,
        jogo character varying(30) NOT NULL,
        carta_externa_id character varying(100) NOT NULL,
        nome character varying(200) NOT NULL,
        numero character varying(20),
        raridade character varying(50),
        imagem_url character varying(500),
        tem boolean NOT NULL,
        quero boolean NOT NULL,
        favorito boolean NOT NULL,
        criado_em timestamp with time zone NOT NULL,
        atualizado_em timestamp with time zone NOT NULL,
        CONSTRAINT "PK_cartas_colecionadas" PRIMARY KEY (id),
        CONSTRAINT "FK_cartas_colecionadas_usuarios_usuario_id" FOREIGN KEY (usuario_id) REFERENCES usuarios (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    CREATE TABLE refresh_tokens (
        id uuid NOT NULL,
        usuario_id uuid NOT NULL,
        token_hash character varying(200) NOT NULL,
        criado_em timestamp with time zone NOT NULL,
        expira_em timestamp with time zone NOT NULL,
        revogado_em timestamp with time zone,
        CONSTRAINT "PK_refresh_tokens" PRIMARY KEY (id),
        CONSTRAINT "FK_refresh_tokens_usuarios_usuario_id" FOREIGN KEY (usuario_id) REFERENCES usuarios (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    CREATE INDEX "IX_cartas_colecionadas_usuario_id_jogo" ON cartas_colecionadas (usuario_id, jogo);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    CREATE UNIQUE INDEX "IX_cartas_colecionadas_usuario_id_jogo_carta_externa_id" ON cartas_colecionadas (usuario_id, jogo, carta_externa_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    CREATE UNIQUE INDEX "IX_refresh_tokens_token_hash" ON refresh_tokens (token_hash);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    CREATE INDEX "IX_refresh_tokens_usuario_id" ON refresh_tokens (usuario_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260725011314_RefinamentoPermissoesEColecao') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260725011314_RefinamentoPermissoesEColecao', '10.0.10');
    END IF;
END $EF$;
COMMIT;

