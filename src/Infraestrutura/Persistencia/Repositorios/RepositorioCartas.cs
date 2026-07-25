using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioCartas : IRepositorioCartas
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioCartas(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<Carta?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        _contexto.Cartas.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<(IReadOnlyList<Carta> Itens, int Total)> ListarAsync(FiltroCartas filtro, CancellationToken ct = default)
    {
        var consulta = _contexto.Cartas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Busca))
        {
            var busca = $"%{filtro.Busca}%";
            consulta = consulta.Where(c => EF.Functions.ILike(c.Nome, busca) || (c.Expansao != null && EF.Functions.ILike(c.Expansao, busca)));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Jogo))
        {
            consulta = consulta.Where(c => c.Jogo == filtro.Jogo);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Raridade))
        {
            consulta = consulta.Where(c => c.Raridade == filtro.Raridade);
        }

        consulta = filtro.OrdenarPor?.ToLowerInvariant() switch
        {
            "numero" => consulta.OrderBy(c => c.Numero),
            "raridade" => consulta.OrderBy(c => c.Raridade),
            _ => consulta.OrderBy(c => c.Nome),
        };

        var total = await consulta.CountAsync(ct);
        var itens = await consulta.Skip((filtro.Pagina - 1) * filtro.TamanhoPagina).Take(filtro.TamanhoPagina).ToListAsync(ct);
        return (itens, total);
    }

    public Task<int> ContarAsync(CancellationToken ct = default) => _contexto.Cartas.CountAsync(ct);

    public async Task AdicionarAsync(Carta carta, CancellationToken ct = default)
    {
        await _contexto.Cartas.AddAsync(carta, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Carta carta, CancellationToken ct = default)
    {
        _contexto.Cartas.Update(carta);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Carta carta, CancellationToken ct = default)
    {
        _contexto.Cartas.Remove(carta);
        await _contexto.SaveChangesAsync(ct);
    }
}
