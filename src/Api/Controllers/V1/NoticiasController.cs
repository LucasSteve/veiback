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
[Route("api/v{version:apiVersion}/noticias")]
public class NoticiasController : ControllerBase
{
    private readonly ServicoNoticias _servico;
    private readonly IUsuarioAutenticado _usuarioAutenticado;

    public NoticiasController(ServicoNoticias servico, IUsuarioAutenticado usuarioAutenticado)
    {
        _servico = servico;
        _usuarioAutenticado = usuarioAutenticado;
    }

    /// <summary>Lista notícias, com paginação e filtro por categoria.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<NoticiaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<NoticiaResponse>>> Listar([FromQuery] string? categoria, [FromQuery] ParametrosPaginacao paginacao, CancellationToken ct)
    {
        var filtro = new FiltroNoticias(categoria, paginacao.Pagina, paginacao.TamanhoPagina);
        return Ok(await _servico.ListarAsync(filtro, ct));
    }

    /// <summary>Obtém uma notícia pelo id, com o conteúdo completo.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NoticiaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoticiaResponse>> ObterPorId(Guid id, CancellationToken ct)
    {
        return Ok(await _servico.ObterPorIdAsync(id, ct));
    }

    /// <summary>Publica uma nova notícia. Requer papel Admin.</summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(NoticiaResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<NoticiaResponse>> Criar(CriarOuAtualizarNoticiaRequest requisicao, CancellationToken ct)
    {
        var resultado = await _servico.CriarAsync(_usuarioAutenticado.UsuarioId!.Value, requisicao, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id, version = "1.0" }, resultado);
    }

    /// <summary>Atualiza uma notícia existente. Requer papel Admin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(NoticiaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<NoticiaResponse>> Atualizar(Guid id, CriarOuAtualizarNoticiaRequest requisicao, CancellationToken ct)
    {
        return Ok(await _servico.AtualizarAsync(id, requisicao, ct));
    }

    /// <summary>Remove uma notícia. Requer papel Admin.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _servico.RemoverAsync(id, ct);
        return NoContent();
    }
}
