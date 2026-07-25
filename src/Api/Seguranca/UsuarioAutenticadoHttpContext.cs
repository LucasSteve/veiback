using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VeiCards.Aplicacao.Portas;

namespace VeiCards.Api.Seguranca;

/// <summary>
/// Implementação da porta IUsuarioAutenticado usando o HttpContext da requisição atual —
/// é o único ponto da aplicação onde "quem está logado" vira algo concreto (claims do JWT).
/// </summary>
public class UsuarioAutenticadoHttpContext : IUsuarioAutenticado
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAutenticadoHttpContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UsuarioId
    {
        get
        {
            var valor = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(valor, out var id) ? id : null;
        }
    }

    public bool EstaAutenticado => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool EhAdmin => _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
}
