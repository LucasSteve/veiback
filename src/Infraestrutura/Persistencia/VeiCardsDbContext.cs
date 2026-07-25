using Microsoft.EntityFrameworkCore;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia;

public class VeiCardsDbContext : DbContext
{
    public VeiCardsDbContext(DbContextOptions<VeiCardsDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<CartaColecionada> CartasColecionadas => Set<CartaColecionada>();
    public DbSet<Noticia> Noticias => Set<Noticia>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<InscricaoEvento> InscricoesEventos => Set<InscricaoEvento>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VeiCardsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
