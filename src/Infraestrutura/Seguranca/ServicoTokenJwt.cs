using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VeiCards.Aplicacao.Portas;
using VeiCards.Dominio.Entidades;
using VeiCards.Infraestrutura.Opcoes;

namespace VeiCards.Infraestrutura.Seguranca;

public class ServicoTokenJwt : IServicoToken
{
    private readonly JwtOpcoes _opcoes;

    public ServicoTokenJwt(IOptions<JwtOpcoes> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public string GerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.NomeUsuario),
            new Claim(ClaimTypes.Role, usuario.Papel.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.ChaveSecreta));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opcoes.Emissor,
            audience: _opcoes.Audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_opcoes.ExpiracaoEmMinutos),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
