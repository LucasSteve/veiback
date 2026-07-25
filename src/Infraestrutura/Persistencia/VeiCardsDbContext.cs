using Microsoft.EntityFrameworkCore;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia;

public class VeiCardsDbContext : DbContext
{
    public VeiCardsDbContext(DbContextOptions<VeiCardsDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Carta> Cartas => Set<Carta>();
    public DbSet<StatusCartaUsuario> StatusCartasUsuario => Set<StatusCartaUsuario>();
    public DbSet<Noticia> Noticias => Set<Noticia>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<InscricaoEvento> InscricoesEventos => Set<InscricaoEvento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VeiCardsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
