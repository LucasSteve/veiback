using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioStatusCartaUsuario
{
    Task<StatusCartaUsuario?> ObterAsync(Guid usuarioId, Guid cartaId, CancellationToken ct = default);
    Task<IReadOnlyList<StatusCartaUsuario>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task AdicionarAsync(StatusCartaUsuario status, CancellationToken ct = default);
    Task AtualizarAsync(StatusCartaUsuario status, CancellationToken ct = default);
}
