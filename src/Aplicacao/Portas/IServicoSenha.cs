namespace VeiCards.Aplicacao.Portas;

/// <summary>
/// Porta para geração/verificação de hash de senha. Implementada em Infraestrutura com BCrypt —
/// a Aplicação não sabe (nem deveria saber) qual algoritmo é usado.
/// </summary>
public interface IServicoSenha
{
    string GerarHash(string senha);
    bool VerificarHash(string senha, string hash);
}
