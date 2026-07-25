using Microsoft.EntityFrameworkCore;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Infraestrutura.Persistencia.Repositorios;

public class RepositorioRefreshTokens : IRepositorioRefreshTokens
{
    private readonly VeiCardsDbContext _contexto;

    public RepositorioRefreshTokens(VeiCardsDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default) =>
        _contexto.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash, ct);

    public async Task AdicionarAsync(RefreshToken token, CancellationToken ct = default)
    {
        await _contexto.RefreshTokens.AddAsync(token, ct);
        await _contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(RefreshToken token, CancellationToken ct = default)
    {
        _contexto.RefreshTokens.Update(token);
        await _contexto.SaveChangesAsync(ct);
    }
}
