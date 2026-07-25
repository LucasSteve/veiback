using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Filtros;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Servicos;
using VeiCards.Dominio.Enums;

namespace VeiCards.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/eventos")]
public class EventosController : ControllerBase
{
    private readonly ServicoEventos _servicoEventos;
    private readonly ServicoInscricoesEventos _servicoInscricoes;
    private readonly IUsuarioAutenticado _usuarioAutenticado;

    public EventosController(ServicoEventos servicoEventos, ServicoInscricoesEventos servicoInscricoes, IUsuarioAutenticado usuarioAutenticado)
    {
        _servicoEventos = servicoEventos;
        _servicoInscricoes = servicoInscricoes;
        _usuarioAutenticado = usuarioAutenticado;
    }

    /// <summary>Lista eventos, com paginação e filtro por cidade/tipo/status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<EventoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<EventoResponse>>> Listar(
        [FromQuery] string? cidade,
        [FromQuery] string? tipo,
        [FromQuery] string? status,
        [FromQuery] ParametrosPaginacao paginacao,
        CancellationToken ct)
    {
        Enum.TryParse<TipoEvento>(tipo, ignoreCase: true, out var tipoEnum);
        Enum.TryParse<StatusEvento>(status, ignoreCase: true, out var statusEnum);

        var filtro = new FiltroEventos(
            cidade,
            string.IsNullOrWhiteSpace(tipo) ? null : tipoEnum,
            string.IsNullOrWhiteSpace(status) ? null : statusEnum,
            paginacao.Pagina,
            paginacao.TamanhoPagina);

        return Ok(await _servicoEventos.ListarAsync(filtro, ct));
    }

    /// <summary>Lista as inscrições do usuário autenticado.</summary>
    [HttpGet("minhas-inscricoes")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<InscricaoEventoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InscricaoEventoResponse>>> ListarMinhasInscricoes(CancellationToken ct)
    {
        return Ok(await _servicoInscricoes.ListarMinhasAsync(_usuarioAutenticado.UsuarioId!.Value, ct));
    }

    /// <summary>Obtém um evento pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoResponse>> ObterPorId(Guid id, CancellationToken ct)
    {
        return Ok(await _servicoEventos.ObterPorIdAsync(id, ct));
    }

    /// <summary>Inscreve o usuário autenticado no evento (idempotente).</summary>
    [HttpPost("{id:guid}/inscricao")]
    [Authorize]
    [ProducesResponseType(typeof(InscricaoEventoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<InscricaoEventoResponse>> Inscrever(Guid id, CancellationToken ct)
    {
        return Ok(await _servicoInscricoes.InscreverAsync(id, _usuarioAutenticado.UsuarioId!.Value, ct));
    }

    /// <summary>Cancela a inscrição do usuário autenticado no evento.</summary>
    [HttpDelete("{id:guid}/inscricao")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelarInscricao(Guid id, CancellationToken ct)
    {
        await _servicoInscricoes.CancelarAsync(id, _usuarioAutenticado.UsuarioId!.Value, ct);
        return NoContent();
    }

    /// <summary>Cria um novo evento. Requer papel Admin.</summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(EventoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<EventoResponse>> Criar(CriarOuAtualizarEventoRequest requisicao, CancellationToken ct)
    {
        var resultado = await _servicoEventos.CriarAsync(requisicao, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id, version = "1.0" }, resultado);
    }

    /// <summary>Atualiza um evento existente. Requer papel Admin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(EventoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventoResponse>> Atualizar(Guid id, CriarOuAtualizarEventoRequest requisicao, CancellationToken ct)
    {
        return Ok(await _servicoEventos.AtualizarAsync(id, requisicao, ct));
    }

    /// <summary>Remove um evento. Requer papel Admin.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _servicoEventos.RemoverAsync(id, ct);
        return NoContent();
    }

    /// <summary>Abre ou fecha as inscrições de um evento. Requer papel Admin.</summary>
    [HttpPatch("{id:guid}/inscricoes-abertas")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(EventoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventoResponse>> AtualizarInscricoesAbertas(Guid id, AtualizarInscricoesAbertasRequest requisicao, CancellationToken ct)
    {
        return Ok(await _servicoEventos.AtualizarInscricoesAbertasAsync(id, requisicao.Abertas, ct));
    }
}
