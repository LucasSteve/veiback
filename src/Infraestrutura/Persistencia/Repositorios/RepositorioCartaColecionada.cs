using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioCartaColecionada : IRepositorioCartaColecionada
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioCartaColecionada(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<CartaColecionada?> ObterAsync(Guid usuarioId, TipoJogo jogo, string cartaExternaId, CancellationToken ct = default) =>
        _contexto.CartasColecionadas.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Jogo == jogo && c.CartaExternaId == cartaExternaId, ct);

    public async Task<(IReadOnlyList<CartaColecionada> Itens, int Total)> ListarAsync(Guid usuarioId, TipoJogo jogo, int pagina, int tamanhoPagina, CancellationToken ct = default)
    {
        var consulta = _contexto.CartasColecionadas
            .Where(c => c.UsuarioId == usuarioId && c.Jogo == jogo)
            .OrderBy(c => c.Nome);

        var total = await consulta.CountAsync(ct);
        var itens = await consulta.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync(ct);
        return (itens, total);
    }

    public async Task<IReadOnlyList<(TipoJogo Jogo, int Quantidade)>> ListarJogosComContagemAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var grupos = await _contexto.CartasColecionadas
            .Where(c => c.UsuarioId == usuarioId)
            .GroupBy(c => c.Jogo)
            .Select(g => new { Jogo = g.Key, Quantidade = g.Count() })
            .ToListAsync(ct);

        return grupos.Select(g => (g.Jogo, g.Quantidade)).ToList();
    }

    public Task<int> ContarAsync(CancellationToken ct = default) => _contexto.CartasColecionadas.CountAsync(ct);

    public async Task AdicionarAsync(CartaColecionada carta, CancellationToken ct = default)
    {
        await _contexto.CartasColecionadas.AddAsync(carta, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(CartaColecionada carta, CancellationToken ct = default)
    {
        _contexto.CartasColecionadas.Update(carta);
        await _contexto.SaveChangesAsync(ct);
    }
}
