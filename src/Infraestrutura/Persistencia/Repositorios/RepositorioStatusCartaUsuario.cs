using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioStatusCartaUsuario : IRepositorioStatusCartaUsuario
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioStatusCartaUsuario(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<StatusCartaUsuario?> ObterAsync(Guid usuarioId, Guid cartaId, CancellationToken ct = default) =>
        _contexto.StatusCartasUsuario.FirstOrDefaultAsync(s => s.UsuarioId == usuarioId && s.CartaId == cartaId, ct);

    public async Task<IReadOnlyList<StatusCartaUsuario>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default) =>
        await _contexto.StatusCartasUsuario.Where(s => s.UsuarioId == usuarioId).ToListAsync(ct);

    public async Task AdicionarAsync(StatusCartaUsuario status, CancellationToken ct = default)
    {
        await _contexto.StatusCartasUsuario.AddAsync(status, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(StatusCartaUsuario status, CancellationToken ct = default)
    {
        _contexto.StatusCartasUsuario.Update(status);
        await _contexto.SaveChangesAsync(ct);
    }
}
