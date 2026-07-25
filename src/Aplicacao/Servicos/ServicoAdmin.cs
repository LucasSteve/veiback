using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>Casos de uso do painel administrativo: métricas e gestão de outros administradores.</summary>
public class ServicoAdmin
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly IRepositorioCartaColecionada _repositorioColecao;
    private readonly IRepositorioNoticias _repositorioNoticias;
    private readonly IRepositorioEventos _repositorioEventos;

    public ServicoAdmin(
        IRepositorioUsuarios repositorioUsuarios,
        IRepositorioCartaColecionada repositorioColecao,
        IRepositorioNoticias repositorioNoticias,
        IRepositorioEventos repositorioEventos)
    {
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioColecao = repositorioColecao;
        _repositorioNoticias = repositorioNoticias;
        _repositorioEventos = repositorioEventos;
    }

    public async Task<EstatisticasResponse> ObterEstatisticasAsync(CancellationToken ct = default)
    {
        var totalUsuarios = await _repositorioUsuarios.ContarAsync(ct);
        var totalCartasColecionadas = await _repositorioColecao.ContarAsync(ct);
        var totalNoticias = await _repositorioNoticias.ContarAsync(ct);
        var totalEventos = await _repositorioEventos.ContarAsync(ct);

        return new EstatisticasResponse(totalUsuarios, totalCartasColecionadas, totalNoticias, totalEventos);
    }

    public async Task<ResultadoPaginado<UsuarioResponse>> ListarUsuariosAsync(int pagina, int tamanhoPagina, CancellationToken ct = default)
    {
        var (itens, total) = await _repositorioUsuarios.ListarAsync(pagina, tamanhoPagina, ct);
        var respostas = itens.Select(MapearParaResponse).ToList();
        return new ResultadoPaginado<UsuarioResponse>(respostas, pagina, tamanhoPagina, total);
    }

    public async Task<UsuarioResponse> PromoverAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var usuario = await _repositorioUsuarios.ObterPorIdAsync(usuarioId, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Usuario), usuarioId);
        usuario.PromoverParaAdmin();
        await _repositorioUsuarios.AtualizarAsync(usuario, ct);
        return MapearParaResponse(usuario);
    }

    public async Task<UsuarioResponse> RebaixarAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var usuario = await _repositorioUsuarios.ObterPorIdAsync(usuarioId, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Usuario), usuarioId);
        usuario.RebaixarParaUsuario();
        await _repositorioUsuarios.AtualizarAsync(usuario, ct);
        return MapearParaResponse(usuario);
    }

    private static UsuarioResponse MapearParaResponse(Usuario u) => new(u.Id, u.NomeUsuario, u.Email, u.NomeExibicao, u.Papel.ToString());
}
