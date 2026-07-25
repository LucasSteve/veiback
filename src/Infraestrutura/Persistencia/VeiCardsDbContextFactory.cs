using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VeiCards.Infraestrutura.Persistencia;

/// <summary>
/// Usada apenas em design-time pelo `dotnet ef migrations` (o host da Api não precisa
/// estar rodando). Lê a connection string da variável de ambiente CONNECTIONSTRINGS__VEICARDSDB,
/// com um fallback local para desenvolvimento sem depender de segredos versionados.
/// </summary>
public class VeiCardsDbContextFactory : IDesignTimeDbContextFactory<VeiCardsDbContext>
{
    public VeiCardsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__VEICARDSDB")
            ?? "Host=localhost;Port=5432;Database=veicards;Username=postgres;Password=postgres";

        var opcoes = new DbContextOptionsBuilder<VeiCardsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new VeiCardsDbContext(opcoes);
    }
}
