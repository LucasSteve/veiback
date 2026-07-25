using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Servicos;
using VeiCards.Dominio.Enums;
using VeiCards.Dominio.Excecoes;

namespace VeiCards.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/colecao")]
[Authorize]
public class ColecaoController : ControllerBase
{
    private readonly ServicoColecaoUsuario _servico;
    private readonly IUsuarioAutenticado _usuarioAutenticado;

    public ColecaoController(ServicoColecaoUsuario servico, IUsuarioAutenticado usuarioAutenticado)
    {
        _servico = servico;
        _usuarioAutenticado = usuarioAutenticado;
    }

    /// <summary>Jogos com pelo menos uma carta colecionada pelo usuário autenticado, com contagem.</summary>
    [HttpGet("jogos")]
    [ProducesResponseType(typeof(IReadOnlyList<JogoComContagemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<JogoComContagemResponse>>> ListarJogos(CancellationToken ct)
    {
        return Ok(await _servico.ListarJogosAsync(_usuarioAutenticado.UsuarioId!.Value, ct));
    }

    /// <summary>Cartas colecionadas de um jogo específico, paginado.</summary>
    [HttpGet("{jogo}")]
    [ProducesResponseType(typeof(ResultadoPaginado<CartaColecionadaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<CartaColecionadaResponse>>> ListarPorJogo(string jogo, [FromQuery] ParametrosPaginacao paginacao, CancellationToken ct)
    {
        var tipoJogo = ConverterJogo(jogo);
        return Ok(await _servico.ListarPorJogoAsync(_usuarioAutenticado.UsuarioId!.Value, tipoJogo, paginacao.Pagina, paginacao.TamanhoPagina, ct));
    }

    /// <summary>Grava (upsert) o status Tenho/Quero/Favorito de uma carta na coleção do usuário autenticado.</summary>
    [HttpPut("{jogo}/{cartaExternaId}")]
    [ProducesResponseType(typeof(CartaColecionadaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartaColecionadaResponse>> AtualizarStatus(string jogo, string cartaExternaId, AtualizarStatusColecaoRequest requisicao, CancellationToken ct)
    {
        var tipoJogo = ConverterJogo(jogo);
        return Ok(await _servico.AtualizarStatusAsync(_usuarioAutenticado.UsuarioId!.Value, tipoJogo, cartaExternaId, requisicao, ct));
    }

    private static TipoJogo ConverterJogo(string jogo)
    {
        if (!Enum.TryParse<TipoJogo>(jogo, ignoreCase: true, out var tipoJogo))
        {
            throw new ExcecaoDeRegraDeNegocio($"Jogo '{jogo}' não é suportado. Valores aceitos: {string.Join(", ", Enum.GetNames<TipoJogo>())}.");
        }

        return tipoJogo;
    }
}
