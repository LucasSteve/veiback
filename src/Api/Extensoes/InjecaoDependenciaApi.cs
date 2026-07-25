using System.Text;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Servicos;
using VeiCards.Aplicacao.Validadores;
using VeiCards.Api.Seguranca;
using VeiCards.Infraestrutura.Opcoes;

namespace VeiCards.Api.Extensoes;

/// <summary>Composition root da camada de Api: autenticação, versionamento, Swagger, health checks.</summary>
public static class InjecaoDependenciaApi
{
    public static IServiceCollection AdicionarServicosDaAplicacao(this IServiceCollection servicos)
    {
        servicos.AddScoped<ServicoAutenticacao>();
        servicos.AddScoped<ServicoColecaoUsuario>();
        servicos.AddScoped<ServicoNoticias>();
        servicos.AddScoped<ServicoEventos>();
        servicos.AddScoped<ServicoInscricoesEventos>();
        servicos.AddScoped<ServicoAdmin>();

        servicos.AddValidatorsFromAssemblyContaining<RegistrarUsuarioRequestValidator>();
        servicos.AddFluentValidationAutoValidation();

        return servicos;
    }

    public static IServiceCollection AdicionarSeguranca(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var jwtOpcoes = configuracao.GetSection(JwtOpcoes.Secao).Get<JwtOpcoes>()
            ?? throw new InvalidOperationException("Seção 'Jwt' não configurada.");

        servicos.AddHttpContextAccessor();
        servicos.AddScoped<IUsuarioAutenticado, UsuarioAutenticadoHttpContext>();

        servicos.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opcoes =>
            {
                // Sem isso, o handler renomeia a claim "sub" para um URI legado
                // (ClaimTypes.NameIdentifier), e IUsuarioAutenticado.UsuarioId — que lê o
                // claim "sub" original emitido por ServicoTokenJwt — sempre voltaria nulo.
                opcoes.MapInboundClaims = false;
                opcoes.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOpcoes.Emissor,
                    ValidAudience = jwtOpcoes.Audiencia,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpcoes.ChaveSecreta)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

        servicos.AddAuthorizationBuilder()
            .AddPolicy("Admin", politica => politica.RequireRole("Admin"));

        return servicos;
    }

    public static IServiceCollection AdicionarVersionamentoDeApi(this IServiceCollection servicos)
    {
        servicos.AddApiVersioning(opcoes =>
        {
            opcoes.DefaultApiVersion = new ApiVersion(1, 0);
            opcoes.AssumeDefaultVersionWhenUnspecified = true;
            opcoes.ReportApiVersions = true;
        }).AddMvc().AddApiExplorer(opcoes =>
        {
            opcoes.GroupNameFormat = "'v'VVV";
            opcoes.SubstituteApiVersionInUrl = true;
        });

        return servicos;
    }

    public static IServiceCollection AdicionarSwagger(this IServiceCollection servicos)
    {
        servicos.AddEndpointsApiExplorer();
        servicos.AddSwaggerGen(opcoes =>
        {
            opcoes.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "VEI Cards API",
                Version = "v1",
                Description = "API REST da plataforma VEI Cards — cartas, coleção, notícias e eventos de TCG.",
            });

            opcoes.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token JWT: Bearer {token}",
            });

            opcoes.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                    Array.Empty<string>()
                },
            });
        });

        return servicos;
    }

    public static IServiceCollection AdicionarCors(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var origensPermitidas = configuracao.GetSection("Cors:OrigensPermitidas").Get<string[]>() ?? [];

        servicos.AddCors(opcoes =>
        {
            opcoes.AddPolicy("PoliticaPadrao", politica =>
            {
                politica.WithOrigins(origensPermitidas)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return servicos;
    }
}
