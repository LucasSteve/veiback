using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>Casos de uso do catálogo de cartas (CRUD administrativo + consulta pública).</summary>
public class ServicoCartas
{
    private readonly IRepositorioCartas _repositorio;

    public ServicoCartas(IRepositorioCartas repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultadoPaginado<CartaResponse>> ListarAsync(FiltroCartas filtro, CancellationToken ct = default)
    {
        var (itens, total) = await _repositorio.ListarAsync(filtro, ct);
        return new ResultadoPaginado<CartaResponse>(itens.Select(MapearParaResponse).ToList(), filtro.Pagina, filtro.TamanhoPagina, total);
    }

    public async Task<CartaResponse> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var carta = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Carta), id);
        return MapearParaResponse(carta);
    }

    public async Task<CartaResponse> CriarAsync(CriarOuAtualizarCartaRequest requisicao, CancellationToken ct = default)
    {
        var carta = Carta.Criar(requisicao.Nome, requisicao.Numero, requisicao.Expansao, requisicao.Raridade, requisicao.Jogo, requisicao.ImagemUrl);
        await _repositorio.AdicionarAsync(carta, ct);
        return MapearParaResponse(carta);
    }

    public async Task<CartaResponse> AtualizarAsync(Guid id, CriarOuAtualizarCartaRequest requisicao, CancellationToken ct = default)
    {
        var carta = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Carta), id);
        carta.Atualizar(requisicao.Nome, requisicao.Numero, requisicao.Expansao, requisicao.Raridade, requisicao.Jogo, requisicao.ImagemUrl);
        await _repositorio.AtualizarAsync(carta, ct);
        return MapearParaResponse(carta);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        var carta = await _repositorio.ObterPorIdAsync(id, ct) ?? throw new ExcecaoDeEntidadeNaoEncontrada(nameof(Carta), id);
        await _repositorio.RemoverAsync(carta, ct);
    }

    private static CartaResponse MapearParaResponse(Carta carta) =>
        new(carta.Id, carta.Nome, carta.Numero, carta.Expansao, carta.Raridade, carta.Jogo, carta.ImagemUrl);
}
