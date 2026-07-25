using VeiCards.Aplicacao.Filtros;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioEventos
{
    Task<Evento?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Evento> Itens, int Total)> ListarAsync(FiltroEventos filtro, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Evento evento, CancellationToken ct = default);
    Task AtualizarAsync(Evento evento, CancellationToken ct = default);
    Task RemoverAsync(Evento evento, CancellationToken ct = default);
}
