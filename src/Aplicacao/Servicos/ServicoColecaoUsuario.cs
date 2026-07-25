using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas.Repositorios;
using VeiCards.Dominio.Entidades;
using VeiCards.Dominio.Enums;

namespace VeiCards.Aplicacao.Servicos;

/// <summary>
/// Casos de uso da coleção pessoal do usuário (Tenho/Quero/Favorito por carta, agrupadas
/// por jogo) — equivalente server-side do collectionStore que hoje vive só no localStorage
/// do frontend. Genérico por design: não conhece nenhuma regra específica de Pokémon,
/// Magic, etc. — só o enum TipoJogo distingue um jogo do outro.
/// </summary>
public class ServicoColecaoUsuario
{
    private readonly IRepositorioCartaColecionada _repositorio;

    public ServicoColecaoUsuario(IRepositorioCartaColecionada repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IReadOnlyList<JogoComContagemResponse>> ListarJogosAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var jogos = await _repositorio.ListarJogosComContagemAsync(usuarioId, ct);
        return jogos.Select(j => new JogoComContagemResponse(j.Jogo.ToString(), j.Quantidade)).ToList();
    }

    public async Task<ResultadoPaginado<CartaColecionadaResponse>> ListarPorJogoAsync(Guid usuarioId, TipoJogo jogo, int pagina, int tamanhoPagina, CancellationToken ct = default)
    {
        var (itens, total) = await _repositorio.ListarAsync(usuarioId, jogo, pagina, tamanhoPagina, ct);
        return new ResultadoPaginado<CartaColecionadaResponse>(itens.Select(MapearParaResponse).ToList(), pagina, tamanhoPagina, total);
    }

    public async Task<CartaColecionadaResponse> AtualizarStatusAsync(Guid usuarioId, TipoJogo jogo, string cartaExternaId, AtualizarStatusColecaoRequest requisicao, CancellationToken ct = default)
    {
        var carta = await _repositorio.ObterAsync(usuarioId, jogo, cartaExternaId, ct);

        if (carta is null)
        {
            carta = CartaColecionada.Criar(usuarioId, jogo, cartaExternaId, requisicao.Nome, requisicao.Numero, requisicao.Raridade, requisicao.ImagemUrl);
            carta.AtualizarStatus(requisicao.Tem, requisicao.Quero, requisicao.Favorito, requisicao.Nome, requisicao.Numero, requisicao.Raridade, requisicao.ImagemUrl);
            await _repositorio.AdicionarAsync(carta, ct);
        }
        else
        {
            carta.AtualizarStatus(requisicao.Tem, requisicao.Quero, requisicao.Favorito, requisicao.Nome, requisicao.Numero, requisicao.Raridade, requisicao.ImagemUrl);
            await _repositorio.AtualizarAsync(carta, ct);
        }

        return MapearParaResponse(carta);
    }

    private static CartaColecionadaResponse MapearParaResponse(CartaColecionada carta) => new(
        carta.Id, carta.Jogo.ToString(), carta.CartaExternaId, carta.Nome, carta.Numero, carta.Raridade, carta.ImagemUrl, carta.Tem, carta.Quero, carta.Favorito);
}
