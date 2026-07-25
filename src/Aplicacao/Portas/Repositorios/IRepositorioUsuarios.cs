using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas.Repositorios;

public interface IRepositorioUsuarios
{
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Usuario?> ObterPorNomeUsuarioAsync(string nomeUsuario, CancellationToken ct = default);
    Task<bool> ExisteComNomeUsuarioOuEmailAsync(string nomeUsuario, string email, CancellationToken ct = default);
    Task<(IReadOnlyList<Usuario> Itens, int Total)> ListarAsync(int pagina, int tamanhoPagina, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct = default);
    Task AtualizarAsync(Usuario usuario, CancellationToken ct = default);
}
