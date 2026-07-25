using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Servicos;

namespace VeiCards.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cartas")]
public class CartasController : ControllerBase
{
    private readonly ServicoCartas _servicoCartas;
    private readonly ServicoColecaoUsuario _servicoColecao;
    private readonly IUsuarioAutenticado _usuarioAutenticado;

    public CartasController(ServicoCartas servicoCartas, ServicoColecaoUsuario servicoColecao, IUsuarioAutenticado usuarioAutenticado)
    {
        _servicoCartas = servicoCartas;
        _servicoColecao = servicoColecao;
        _usuarioAutenticado = usuarioAutenticado;
    }

    /// <summary>Lista cartas do catálogo, com paginação, filtro e ordenação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<CartaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<CartaResponse>>> Listar(
        [FromQuery] string? busca,
        [FromQuery] string? jogo,
        [FromQuery] string? raridade,
        [FromQuery] string? ordenarPor,
        [FromQuery] ParametrosPaginacao paginacao,
        CancellationToken ct)
    {
        var filtro = new FiltroCartas(busca, jogo, raridade, ordenarPor, paginacao.Pagina, paginacao.TamanhoPagina);
        return Ok(await _servicoCartas.ListarAsync(filtro, ct));
    }

    /// <summary>Retorna o status de coleção (Tenho/Quero/Favorito) do usuário autenticado para todas as cartas.</summary>
    [HttpGet("status")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<StatusCartaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StatusCartaResponse>>> ObterStatusDoUsuario(CancellationToken ct)
    {
        return Ok(await _servicoColecao.ObterStatusDoUsuarioAsync(_usuarioAutenticado.UsuarioId!.Value, ct));
    }

    /// <summary>Obtém uma carta pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CartaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartaResponse>> ObterPorId(Guid id, CancellationToken ct)
    {
        return Ok(await _servicoCartas.ObterPorIdAsync(id, ct));
    }

    /// <summary>Alterna o status Tenho/Quero/Favorito de uma carta para o usuário autenticado.</summary>
    [HttpPut("{id:guid}/status")]
    [Authorize]
    [ProducesResponseType(typeof(StatusCartaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatusCartaResponse>> AtualizarStatus(Guid id, AtualizarStatusCartaRequest requisicao, CancellationToken ct)
    {
        return Ok(await _servicoColecao.AtualizarStatusAsync(_usuarioAutenticado.UsuarioId!.Value, id, requisicao, ct));
    }

    /// <summary>Cria uma nova carta no catálogo. Requer papel Admin.</summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(CartaResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CartaResponse>> Criar(CriarOuAtualizarCartaRequest requisicao, CancellationToken ct)
    {
        var resultado = await _servicoCartas.CriarAsync(requisicao, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id, version = "1.0" }, resultado);
    }

    /// <summary>Atualiza uma carta existente. Requer papel Admin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(CartaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartaResponse>> Atualizar(Guid id, CriarOuAtualizarCartaRequest requisicao, CancellationToken ct)
    {
        return Ok(await _servicoCartas.AtualizarAsync(id, requisicao, ct));
    }

    /// <summary>Remove uma carta do catálogo. Requer papel Admin.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _servicoCartas.RemoverAsync(id, ct);
        return NoContent();
    }
}
