using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas.Repositorios;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>Casos de uso do painel administrativo — espelha o AdminPanel do frontend.</summary>
public class ServicoAdmin
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly IRepositorioCartas _repositorioCartas;
    private readonly IRepositorioNoticias _repositorioNoticias;
    private readonly IRepositorioEventos _repositorioEventos;

    public ServicoAdmin(
        IRepositorioUsuarios repositorioUsuarios,
        IRepositorioCartas repositorioCartas,
        IRepositorioNoticias repositorioNoticias,
        IRepositorioEventos repositorioEventos)
    {
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioCartas = repositorioCartas;
        _repositorioNoticias = repositorioNoticias;
        _repositorioEventos = repositorioEventos;
    }

    public async Task<EstatisticasResponse> ObterEstatisticasAsync(CancellationToken ct = default)
    {
        var totalUsuarios = await _repositorioUsuarios.ContarAsync(ct);
        var totalCartas = await _repositorioCartas.ContarAsync(ct);
        var totalNoticias = await _repositorioNoticias.ContarAsync(ct);
        var totalEventos = await _repositorioEventos.ContarAsync(ct);

        return new EstatisticasResponse(totalUsuarios, totalCartas, totalNoticias, totalEventos);
    }

    public async Task<ResultadoPaginado<UsuarioResponse>> ListarUsuariosAsync(int pagina, int tamanhoPagina, CancellationToken ct = default)
    {
        var (itens, total) = await _repositorioUsuarios.ListarAsync(pagina, tamanhoPagina, ct);
        var respostas = itens.Select(u => new UsuarioResponse(u.Id, u.NomeUsuario, u.Email, u.NomeExibicao, u.Papel.ToString())).ToList();
        return new ResultadoPaginado<UsuarioResponse>(respostas, pagina, tamanhoPagina, total);
    }
}
