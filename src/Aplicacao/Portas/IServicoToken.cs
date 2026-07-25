using VeiCards.Dominio.Entidades;

namespace VeiCards.Aplicacao.Portas;

/// <summary>Porta para emissão de tokens de acesso e refresh. Implementada em Infraestrutura.</summary>
public interface IServicoToken
{
    (string Token, DateTime ExpiraEm) GerarTokenDeAcesso(Usuario usuario);

    /// <summary>Gera um valor aleatório opaco para ser entregue ao cliente como refresh token.</summary>
    string GerarRefreshTokenBruto();

    /// <summary>Hash de um refresh token para armazenamento — nunca guardamos o valor bruto.</summary>
    string CalcularHashRefreshToken(string tokenBruto);

    TimeSpan DuracaoRefreshToken { get; }
}
