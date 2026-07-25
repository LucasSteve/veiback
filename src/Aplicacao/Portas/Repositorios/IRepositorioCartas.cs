using VeiCards.Aplicacao.Filtros;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioCartas
{
    Task<Carta?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Carta> Itens, int Total)> ListarAsync(FiltroCartas filtro, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Carta carta, CancellationToken ct = default);
    Task AtualizarAsync(Carta carta, CancellationToken ct = default);
    Task RemoverAsync(Carta carta, CancellationToken ct = default);
}
