using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioRefreshTokens
{
    Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken ct = default);
    Task AdicionarAsync(RefreshToken token, CancellationToken ct = default);
    Task AtualizarAsync(RefreshToken token, CancellationToken ct = default);
}
