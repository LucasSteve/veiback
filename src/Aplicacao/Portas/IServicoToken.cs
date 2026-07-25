using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas;

/// <summary>Porta para emissão do token JWT. Implementada em Infraestrutura.</summary>
public interface IServicoToken
{
    string GerarToken(Usuario usuario);
}
