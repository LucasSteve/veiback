using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeiCards.Aplicacao.Dtos;
using VeiCards.Aplicacao.Portas;
using VeiCards.Aplicacao.Servicos;

namespace VeiCards.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/autenticacao")]
public class AutenticacaoController : ControllerBase
{
    private readonly ServicoAutenticacao _servico;
    private readonly IUsuarioAutenticado _usuarioAutenticado;

    public AutenticacaoController(ServicoAutenticacao servico, IUsuarioAutenticado usuarioAutenticado)
    {
        _servico = servico;
        _usuarioAutenticado = usuarioAutenticado;
    }

    /// <summary>Cria uma nova conta de usuário e retorna o par de tokens (acesso + refresh).</summary>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(AutenticacaoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AutenticacaoResponse>> Registrar(RegistrarUsuarioRequest requisicao, CancellationToken ct)
    {
        var resultado = await _servico.RegistrarAsync(requisicao, ct);
        return Ok(resultado);
    }

    /// <summary>Autentica um usuário existente e retorna o par de tokens (acesso + refresh).</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AutenticacaoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AutenticacaoResponse>> Login(LoginRequest requisicao, CancellationToken ct)
    {
        var resultado = await _servico.LoginAsync(requisicao, ct);
        return Ok(resultado);
    }

    /// <summary>
    /// Troca um refresh token válido por um novo par de tokens (rotação: o token
    /// apresentado é revogado, mesmo que ainda não tenha expirado).
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AutenticacaoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AutenticacaoResponse>> RenovarSessao(RefreshTokenRequest requisicao, CancellationToken ct)
    {
        var resultado = await _servico.RenovarSessaoAsync(requisicao, ct);
        return Ok(resultado);
    }

    /// <summary>Retorna o perfil do usuário autenticado.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioResponse>> ObterPerfil(CancellationToken ct)
    {
        var resultado = await _servico.ObterPerfilAsync(_usuarioAutenticado.UsuarioId!.Value, ct);
        return Ok(resultado);
    }

    /// <summary>Atualiza o perfil (nome de exibição, email) do usuário autenticado.</summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioResponse>> AtualizarPerfil(AtualizarPerfilRequest requisicao, CancellationToken ct)
    {
        var resultado = await _servico.AtualizarPerfilAsync(_usuarioAutenticado.UsuarioId!.Value, requisicao, ct);
        return Ok(resultado);
    }
}
