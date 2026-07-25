using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioNoticias : IRepositorioNoticias
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioNoticias(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<Noticia?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        _contexto.Noticias.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<(IReadOnlyList<Noticia> Itens, int Total)> ListarAsync(FiltroNoticias filtro, CancellationToken ct = default)
    {
        var consulta = _contexto.Noticias.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
        {
            consulta = consulta.Where(n => n.Categoria == filtro.Categoria);
        }

        consulta = consulta.OrderByDescending(n => n.DataPublicacao);

        var total = await consulta.CountAsync(ct);
        var itens = await consulta.Skip((filtro.Pagina - 1) * filtro.TamanhoPagina).Take(filtro.TamanhoPagina).ToListAsync(ct);
        return (itens, total);
    }

    public Task<int> ContarAsync(CancellationToken ct = default) => _contexto.Noticias.CountAsync(ct);

    public async Task AdicionarAsync(Noticia noticia, CancellationToken ct = default)
    {
        await _contexto.Noticias.AddAsync(noticia, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Noticia noticia, CancellationToken ct = default)
    {
        _contexto.Noticias.Update(noticia);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Noticia noticia, CancellationToken ct = default)
    {
        _contexto.Noticias.Remove(noticia);
        await _contexto.SaveChangesAsync(ct);
    }
}
