using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioInscricoesEventos : IRepositorioInscricoesEventos
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioInscricoesEventos(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<InscricaoEvento?> ObterAsync(Guid eventoId, Guid usuarioId, CancellationToken ct = default) =>
        _contexto.InscricoesEventos.FirstOrDefaultAsync(i => i.EventoId == eventoId && i.UsuarioId == usuarioId, ct);

    public Task<int> ContarPorEventoAsync(Guid eventoId, CancellationToken ct = default) =>
        _contexto.InscricoesEventos.CountAsync(i => i.EventoId == eventoId, ct);

    public async Task<IReadOnlyList<InscricaoEvento>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default) =>
        await _contexto.InscricoesEventos.Where(i => i.UsuarioId == usuarioId).ToListAsync(ct);

    public async Task AdicionarAsync(InscricaoEvento inscricao, CancellationToken ct = default)
    {
        await _contexto.InscricoesEventos.AddAsync(inscricao, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(InscricaoEvento inscricao, CancellationToken ct = default)
    {
        _contexto.InscricoesEventos.Remove(inscricao);
        await _contexto.SaveChangesAsync(ct);
    }
}
