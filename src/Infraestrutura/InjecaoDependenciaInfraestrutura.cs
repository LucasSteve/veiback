using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Infraestrutura.Opcoes;
using VeiCards.Infraestrutura.Persistencia;
using VeiCards.Infraestrutura.Persistencia.Repositorios;
using VeiCards.Infraestrutura.Seguranca;

namespace VeiCards.Infraestrutura;

/// <summary>Composition root da camada de Infraestrutura: registra tudo que implementa as portas da Aplicação.</summary>
public static class InjecaoDependenciaInfraestrutura
{
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var connectionString = configuracao.GetConnectionString("VeiCardsDb")
            ?? throw new InvalidOperationException("Connection string 'VeiCardsDb' não configurada.");

        servicos.AddDbContext<VeiCardsDbContext>(opcoes => opcoes.UseNpgsql(connectionString));

        servicos.Configure<JwtOpcoes>(configuracao.GetSection(JwtOpcoes.Secao));

        servicos.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
        servicos.AddScoped<IRepositorioCartas, RepositorioCartas>();
        servicos.AddScoped<IRepositorioStatusCartaUsuario, RepositorioStatusCartaUsuario>();
        servicos.AddScoped<IRepositorioNoticias, RepositorioNoticias>();
        servicos.AddScoped<IRepositorioEventos, RepositorioEventos>();
        servicos.AddScoped<IRepositorioInscricoesEventos, RepositorioInscricoesEventos>();

        servicos.AddScoped<IServicoSenha, ServicoSenhaBCrypt>();
        servicos.AddScoped<IServicoToken, ServicoTokenJwt>();

        return servicos;
    }
}
