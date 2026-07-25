# VEI Cards — Backend

API REST em .NET 10 / ASP.NET Core para a plataforma VEI Cards, construída em Arquitetura
Hexagonal (Ports & Adapters). Este backend é **completamente independente** do frontend
(`../src`) — nenhum arquivo do frontend foi alterado. A integração acontece apenas via HTTP,
documentada na seção [Endpoints](#endpoints).

## ⚠️ Sobre a connection string

A connection string do Supabase compartilhada na conversa **não foi commitada em nenhum
arquivo** — ela contém a senha do banco em texto puro. `appsettings.json` traz os campos
`ConnectionStrings:VeiCardsDb` e `Jwt:ChaveSecreta` vazios de propósito; forneça os valores
reais via variável de ambiente ou `dotnet user-secrets` (ver abaixo). **Recomendo fortemente
rotacionar a senha do Supabase**, já que ela foi exposta em texto puro nesta conversa.

## Arquitetura

```
backend/
├── src/
│   ├── Dominio/          Entidades, enums e regras de negócio puras. Zero dependências.
│   ├── Aplicacao/        Casos de uso, DTOs, portas (interfaces) e validadores FluentValidation.
│   ├── Infraestrutura/   EF Core (DbContext, Configurations, Migrations), repositórios, JWT, BCrypt.
│   └── Api/               Controllers, middlewares, Program.cs, appsettings.
└── testes/
    ├── Dominio.Testes/         Testes unitários de entidades/regras de negócio.
    ├── Aplicacao.Testes/       Testes unitários de casos de uso, com Moq.
    └── Infraestrutura.Testes/  Testes de integração (WebApplicationFactory) dos principais endpoints.
```

As interfaces de repositório (portas) ficam em `Aplicacao/Portas`, não em `Dominio` — é a
Aplicação quem declara o que precisa do mundo externo; o Domínio permanece 100% livre de
qualquer dependência de infraestrutura. Não existe Unit of Work separado: o próprio
`DbContext` do EF Core já cumpre esse papel (cada método de repositório que muda estado
chama `SaveChangesAsync`). Um repositório por agregado, sem abstração genérica — decisões
detalhadas na conversa que precedeu a implementação.

## Como rodar

### Opção 1 — Docker Compose (recomendado)

```bash
cp .env.example .env
# edite .env com uma senha de Postgres e uma chave JWT de verdade
docker compose up --build
```

API disponível em `http://localhost:8080`, Swagger em `http://localhost:8080/swagger` (ambiente Development).

### Opção 2 — Local com .NET SDK

```bash
# defina a connection string e o segredo JWT (não vão para appsettings.json)
export ConnectionStrings__VeiCardsDb="Host=localhost;Port=5432;Database=veicards;Username=postgres;Password=postgres"
export Jwt__ChaveSecreta="uma-chave-bem-grande-e-aleatoria"

dotnet ef database update --project src/Infraestrutura --startup-project src/Infraestrutura
dotnet run --project src/Api
```

Ou, em desenvolvimento, use `dotnet user-secrets` dentro de `src/Api` em vez de variáveis de ambiente.

### Testes

```bash
dotnet test
```

29 testes (13 domínio, 10 aplicação, 6 integração) — os testes de integração usam o
provider InMemory do EF Core, não exigem PostgreSQL rodando.

### Migrations

A migration inicial já está gerada em `src/Infraestrutura/Persistencia/Migracoes/`, e o
script SQL equivalente em `scripts/script-inicial-banco.sql`. Para gerar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigracao --project src/Infraestrutura --startup-project src/Infraestrutura --output-dir Persistencia/Migracoes
```

## Modelo de dados

| Tabela | Descrição |
|---|---|
| `usuarios` | Conta de usuário (username/email únicos, senha com hash BCrypt, papel Usuario/Admin) |
| `cartas` | Catálogo interno de cartas (independente da integração TCGdex do frontend) |
| `status_cartas_usuario` | Tenho/Quero/Favorito por usuário+carta (único por par) |
| `noticias` | Notícias, com autor referenciando `usuarios` |
| `eventos` | Eventos (torneio/liga/prerelease/campeonato/encontro) — status é **calculado**, não armazenado |
| `inscricoes_eventos` | Inscrição de um usuário em um evento (único por par) |

## Endpoints

Base: `/api/v1`. Autenticação via `Authorization: Bearer {token}`. Documentação interativa completa em `/swagger`.

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| POST | `/auth/registrar` | — | Cria conta e retorna token |
| POST | `/auth/login` | — | Autentica e retorna token |
| GET | `/auth/me` | usuário | Perfil autenticado |
| PUT | `/auth/me` | usuário | Atualiza nome/email |
| GET | `/cartas` | — | Lista cartas (paginação, filtros `busca`/`jogo`/`raridade`, `ordenarPor`) |
| GET | `/cartas/{id}` | — | Detalhe de uma carta |
| GET | `/cartas/status` | usuário | Status de coleção do usuário para todas as cartas |
| PUT | `/cartas/{id}/status` | usuário | Atualiza Tenho/Quero/Favorito |
| POST/PUT/DELETE | `/cartas...` | admin | CRUD do catálogo |
| GET | `/noticias` | — | Lista notícias (paginação, filtro `categoria`) |
| GET | `/noticias/{id}` | — | Notícia completa |
| POST/PUT/DELETE | `/noticias...` | admin | CRUD de notícias |
| GET | `/eventos` | — | Lista eventos (paginação, filtros `cidade`/`tipo`/`status`) |
| GET | `/eventos/{id}` | — | Detalhe do evento |
| POST | `/eventos/{id}/inscricao` | usuário | Inscreve-se (idempotente, respeita capacidade) |
| DELETE | `/eventos/{id}/inscricao` | usuário | Cancela inscrição |
| GET | `/eventos/minhas-inscricoes` | usuário | Inscrições do usuário autenticado |
| POST/PUT/DELETE | `/eventos...` | admin | CRUD de eventos |
| GET | `/admin/estatisticas` | admin | Contagens gerais (espelha o AdminPanel do frontend) |
| GET | `/admin/usuarios` | admin | Lista usuários |
| GET | `/health` | — | Health check (verifica conectividade com PostgreSQL) |

## Integração com o frontend (apenas documentação — nenhum código do frontend foi alterado)

O frontend hoje usa dados mockados (`src/services/mockApi.ts` / `src/mocks/*`) com o mesmo
formato conceitual das entidades acima. Para integrar de verdade, bastaria trocar
`src/services/api.ts` (já existe, já é usado quando `VITE_USE_MOCK_API=false` e
`VITE_API_BASE_URL` aponta para este backend) para bater nas rotas desta API — nenhuma
mudança estrutural seria necessária no frontend além de mapear os nomes de campo
(a maioria já bate: `id`, `name`→`Nome`, etc. exigiriam um pequeno adaptador de shape).

## O que ficou fora do escopo (e por quê)

- **TCGdex**: é uma integração pública de terceiros já resolvida inteiramente no frontend
  (chamada direta do browser); trazê-la para o backend não tinha justificativa clara nesta
  fase.
- **Troca de senha / recuperação de senha**: não existe no frontend atual; não implementado
  para não expandir escopo sem pedido explícito.
