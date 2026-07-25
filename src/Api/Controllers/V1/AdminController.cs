using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Servicos;

namespace VeiCards.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Policy = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ServicoAdmin _servico;

    public AdminController(ServicoAdmin servico)
    {
        _servico = servico;
    }

    /// <summary>Estatísticas gerais da plataforma (contagem de usuários, cartas, notícias e eventos).</summary>
    [HttpGet("estatisticas")]
    [ProducesResponseType(typeof(EstatisticasResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<EstatisticasResponse>> ObterEstatisticas(CancellationToken ct)
    {
        return Ok(await _servico.ObterEstatisticasAsync(ct));
    }

    /// <summary>Lista usuários cadastrados, paginado.</summary>
    [HttpGet("usuarios")]
    [ProducesResponseType(typeof(ResultadoPaginado<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<UsuarioResponse>>> ListarUsuarios([FromQuery] ParametrosPaginacao paginacao, CancellationToken ct)
    {
        return Ok(await _servico.ListarUsuariosAsync(paginacao.Pagina, paginacao.TamanhoPagina, ct));
    }
}
