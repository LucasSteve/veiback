using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioCartaColecionada
{
    Task<CartaColecionada?> ObterAsync(Guid usuarioId, TipoJogo jogo, string cartaExternaId, CancellationToken ct = default);
    Task<(IReadOnlyList<CartaColecionada> Itens, int Total)> ListarAsync(Guid usuarioId, TipoJogo jogo, int pagina, int tamanhoPagina, CancellationToken ct = default);
    Task<IReadOnlyList<(TipoJogo Jogo, int Quantidade)>> ListarJogosComContagemAsync(Guid usuarioId, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    Task AdicionarAsync(CartaColecionada carta, CancellationToken ct = default);
    Task AtualizarAsync(CartaColecionada carta, CancellationToken ct = default);
}
