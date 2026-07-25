using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioInscricoesEventos
{
    Task<InscricaoEvento?> ObterAsync(Guid eventoId, Guid usuarioId, CancellationToken ct = default);
    Task<int> ContarPorEventoAsync(Guid eventoId, CancellationToken ct = default);
    Task<IReadOnlyList<InscricaoEvento>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task AdicionarAsync(InscricaoEvento inscricao, CancellationToken ct = default);
    Task RemoverAsync(InscricaoEvento inscricao, CancellationToken ct = default);
}
