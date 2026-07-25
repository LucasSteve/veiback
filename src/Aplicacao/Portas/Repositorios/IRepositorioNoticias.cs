using VeiCards.Aplicacao.Filtros;
using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioNoticias
{
    Task<Noticia?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Noticia> Itens, int Total)> ListarAsync(FiltroNoticias filtro, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Noticia noticia, CancellationToken ct = default);
    Task AtualizarAsync(Noticia noticia, CancellationToken ct = default);
    Task RemoverAsync(Noticia noticia, CancellationToken ct = default);
}
