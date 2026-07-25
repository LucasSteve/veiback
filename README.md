# VEI Cards — Backend

API REST em .NET 10 / ASP.NET Core para a plataforma VEI Cards, construída em Arquitetura
Hexagonal (Ports & Adapters). Vive em repositório próprio, separado do frontend — a
integração acontece só via HTTP.

## ⚠️ Sobre segredos

`appsettings.json` traz `ConnectionStrings:VeiCardsDb` e `Jwt:ChaveSecreta` vazios de
propósito — nunca commite valores reais nesses campos. Forneça-os via variável de ambiente,
`dotnet user-secrets` ou o arquivo `.env` (gitignored) usado pelo Docker Compose.

## Arquitetura

```
backend/
├── src/
│   ├── Dominio/          Entidades, enums e regras de negócio puras. Zero dependências.
│   ├── Aplicacao/        Casos de uso, DTOs, portas (interfaces) e validadores FluentValidation.
│   ├── Infraestrutura/   EF Core (DbContext, Configurations, Migrations), repositórios, JWT, BCrypt, seed inicial.
│   └── Api/               Controllers, middlewares, Program.cs, appsettings.
└── testes/
    ├── Dominio.Testes/         Testes unitários de entidades/regras de negócio.
    ├── Aplicacao.Testes/       Testes unitários de casos de uso, com Moq.
    └── Infraestrutura.Testes/  Testes de integração (WebApplicationFactory) dos principais endpoints.
```

Interfaces de repositório (portas) ficam em `Aplicacao/Portas` — a Aplicação declara o que
precisa do mundo externo; o Domínio permanece livre de infraestrutura. Sem Unit of Work
separado: o `DbContext` já cumpre esse papel. Um repositório por agregado, sem abstração
genérica.

## Controle de acesso (Roles)

Dois papéis: `Usuario` (padrão) e `Admin`. Endpoints administrativos exigem
`[Authorize(Policy = "Admin")]` — usuários comuns recebem 403. Um administrador consegue
promover/rebaixar outros usuários via `/admin/usuarios/{id}/promover` e `/rebaixar`.

### Usuário admin inicial (seed)

Na primeira execução, a API cria automaticamente (se ainda não existir) um usuário:

- **Usuário**: `admin`
- **Senha**: `Abc#123`

A verificação é idempotente (por `nomeUsuario`) — rodar a API várias vezes não duplica nem
recria o admin. Senha armazenada com hash BCrypt, como qualquer outro usuário.

## Como rodar

### Opção 1 — Docker Compose (recomendado)

```bash
cp .env.example .env
# edite .env com uma senha de Postgres e uma chave JWT de verdade
docker compose up --build
```

API em `http://localhost:8080`, Swagger em `http://localhost:8080/swagger` (ambiente Development).

### Opção 2 — Local com .NET SDK

```bash
export ConnectionStrings__VeiCardsDb="Host=localhost;Port=5432;Database=veicards;Username=postgres;Password=postgres"
export Jwt__ChaveSecreta="uma-chave-bem-grande-e-aleatoria"

dotnet run --project src/Api
```

Migrations e o seed do admin rodam automaticamente no startup (`Program.cs`) — não é
necessário nenhum passo manual além de configurar a connection string.

### Testes

```bash
dotnet test
```

42 testes (domínio, aplicação com Moq, integração via `WebApplicationFactory` + EF Core InMemory).

### Migrations

```bash
dotnet ef migrations add NomeDaMigracao --project src/Infraestrutura --startup-project src/Infraestrutura --output-dir Persistencia/Migracoes
```

O script SQL consolidado das migrations vigentes está em `scripts/script-inicial-banco.sql`.

## Modelo de dados

| Tabela | Descrição |
|---|---|
| `usuarios` | Conta de usuário (username/email únicos, senha com hash BCrypt, papel Usuario/Admin) |
| `cartas_colecionadas` | Cartas salvas na coleção pessoal de cada usuário — **snapshot completo** (nome, número, raridade, imagem), não uma referência a um catálogo. Genérico por jogo (enum `TipoJogo`), sem código específico de nenhum TCG |
| `refresh_tokens` | Sessões de longa duração, com rotação (cada uso revoga o token e emite um novo) |
| `noticias` | Notícias, com autor referenciando `usuarios` |
| `eventos` | Eventos (torneio/liga/prerelease/campeonato/encontro). `status` é **calculado** a partir da data (nunca fica desatualizado); `inscricoes_abertas` controla se o botão de inscrição aparece no frontend |
| `inscricoes_eventos` | Inscrição de um usuário em um evento (único por par) |

A coleção foi desenhada para crescer sem alterações estruturais: adicionar um jogo novo é
só adicionar um valor ao enum `TipoJogo` — nenhuma tabela ou código específico por jogo.

## Endpoints

Base: `/api/v1`. Autenticação via `Authorization: Bearer {token}`. Documentação interativa
completa (com botão **Authorize** para JWT) em `/swagger`.

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| POST | `/autenticacao/registrar` | — | Cria conta, retorna token + refreshToken |
| POST | `/autenticacao/login` | — | Autentica, retorna token + refreshToken |
| POST | `/autenticacao/refresh` | — | Troca um refresh token válido por um par novo (rotação) |
| GET | `/autenticacao/me` | usuário | Perfil autenticado |
| PUT | `/autenticacao/me` | usuário | Atualiza nome/email |
| GET | `/colecao/jogos` | usuário | Jogos com pelo menos 1 carta colecionada, com contagem |
| GET | `/colecao/{jogo}` | usuário | Cartas colecionadas de um jogo (paginado) |
| PUT | `/colecao/{jogo}/{cartaExternaId}` | usuário | Upsert de Tenho/Quero/Favorito (grava o snapshot) |
| GET | `/noticias` | — | Lista notícias (paginação, filtro `categoria`) |
| GET | `/noticias/{id}` | — | Notícia completa |
| POST/PUT/DELETE | `/noticias...` | admin | CRUD de notícias |
| GET | `/eventos` | — | Lista eventos (paginação, filtros `cidade`/`tipo`/`status`) |
| GET | `/eventos/{id}` | — | Detalhe do evento |
| POST | `/eventos/{id}/inscricao` | usuário | Inscreve-se (idempotente, respeita capacidade e `inscricoesAbertas`) |
| DELETE | `/eventos/{id}/inscricao` | usuário | Cancela inscrição |
| GET | `/eventos/minhas-inscricoes` | usuário | Inscrições do usuário autenticado |
| POST/PUT/DELETE | `/eventos...` | admin | CRUD de eventos |
| PATCH | `/eventos/{id}/inscricoes-abertas` | admin | Abre/fecha inscrições do evento |
| GET | `/admin/estatisticas` | admin | Contagens gerais (usuários, cartas colecionadas, notícias, eventos) |
| GET | `/admin/usuarios` | admin | Lista usuários |
| PUT | `/admin/usuarios/{id}/promover` | admin | Promove usuário a admin |
| PUT | `/admin/usuarios/{id}/rebaixar` | admin | Rebaixa admin a usuário comum |
| GET | `/health` | — | Health check (fora do prefixo `/api/v1`) |

## Testando pelo Swagger

1. `dotnet run --project src/Api` (ou `docker compose up`).
2. Abra `http://localhost:8080/swagger`.
3. Rode `POST /autenticacao/login` com `{"nomeUsuario":"admin","senha":"Abc#123"}`.
4. Copie o campo `token` da resposta.
5. Clique em **Authorize** (canto superior direito), cole `Bearer {token}`.
6. Todos os endpoints protegidos ficam testáveis diretamente na página.

## Testando pelo Postman

Arquivos em `postman/`:

- `VeiCards.postman_collection.json` — todos os endpoints, organizados em pastas (Autenticação, Coleção, Eventos, Notícias, Administração, Infra).
- `VeiCards.postman_environment.json` — variáveis `baseUrl`, `token`, `refreshToken`, `usuarioAdmin`, `senhaAdmin`, etc.

Importe os dois no Postman, selecione o Environment "VEI Cards - Local", rode **Autenticação
→ Login (admin)** — o token é salvo automaticamente em `{{token}}` (via script de teste) e
reutilizado por todo o resto da collection sem nenhuma configuração adicional. O mesmo vale
para `{{refreshToken}}`, `{{eventoId}}`, `{{noticiaId}}` e `{{usuarioId}}`, preenchidos
automaticamente pelas requisições que os criam.

Validado de ponta a ponta com Newman (`npx newman run postman/VeiCards.postman_collection.json -e postman/VeiCards.postman_environment.json`) — 27 requisições, 0 falhas.

## Integração com o frontend

O frontend (`VeicardsFront`, repositório separado) já está conectado a esta API via
`src/services/api.ts` — sem mocks, sem gambiarra. Para apontar o frontend para uma instância
diferente do backend, defina no `.env.local` dele:

```
VITE_USE_MOCK_API=false
VITE_API_BASE_URL=http://localhost:8080/api/v1
```

## O que ficou fora do escopo (e por quê)

- **TCGdex, Scryfall, YGOPRODeck etc.**: integrações públicas de terceiros consumidas
  diretamente pelo frontend (browser) — o backend não faz proxy delas, evita acoplar nossa
  disponibilidade à de terceiros.
- **Navegação de cartas para jogos além de Pokémon**: a arquitetura da coleção já suporta
  qualquer jogo (basta adicionar ao enum `TipoJogo`), mas a navegação client-side via API
  externa só está implementada para Pokémon por enquanto — os demais jogos aparecem no
  seletor com aviso "em breve".
