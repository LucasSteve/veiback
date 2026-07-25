using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VeiCards.Infraestrutura.Persistencia;

namespace VeiCards.Infraestrutura.Testes;

/// <summary>
/// Sobe a Api real (Program.cs, DI, middlewares, controllers) em memória para testes de
/// integração, substituindo apenas o provider do EF Core (Npgsql -> InMemory) para não
/// depender de um PostgreSQL real rodando durante a build.
/// </summary>
public class FabricaApiTestes : WebApplicationFactory<Program>
{
    // Um único IServiceProvider interno do EF, compartilhado por todas as instâncias de
    // DbContext criadas por esta factory — sem isso, cada requisição HTTP (cada escopo de
    // DI) monta seu próprio provider interno e enxerga um banco InMemory isolado mesmo
    // usando o mesmo nome, fazendo dados gravados numa requisição "sumirem" na próxima.
    private readonly IServiceProvider _provedorInternoEfCore = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .BuildServiceProvider();

    private readonly string _nomeDoBanco = $"veicards-testes-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(servicos =>
        {
            // Remove todo o registro de EF Core feito pela Infraestrutura (Npgsql + DbContext),
            // incluindo os serviços internos de provider (IDatabaseProvider), para então
            // registrar o VeiCardsDbContext de novo, mas com o provider InMemory.
            var descritoresParaRemover = servicos
                .Where(d => (d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") ?? false)
                    || (d.ServiceType.Assembly.GetName().Name?.Contains("Npgsql") ?? false)
                    || d.ServiceType == typeof(VeiCardsDbContext))
                .ToList();

            foreach (var descritor in descritoresParaRemover)
            {
                servicos.Remove(descritor);
            }

            servicos.AddDbContext<VeiCardsDbContext>(opcoes => opcoes
                .UseInMemoryDatabase(_nomeDoBanco)
                .UseInternalServiceProvider(_provedorInternoEfCore));
        });
    }
}
